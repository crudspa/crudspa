using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace Crudspa.Framework.Core.Server.Services;

public class CacheServiceMemory(IMemoryCache cache) : ICacheService
{
    private readonly ConcurrentDictionary<String, Lazy<Task<Object?>>> _gets = new(StringComparer.Ordinal);

    public async Task<Response<T>> GetOrAdd<T>(String key, Func<Task<Response<T>>> fetch, TimeSpan? expiration = null) where T : class?
    {
        var response = Get<T>(key);

        if (response is not null)
            return response;

        response = await Run(Tag<Response<T>>(key), async () => (Response<T>?)await fetch()) ?? new();

        if (response.Ok)
            Add(key, response, expiration ?? TimeSpan.FromDays(1));

        return response;
    }

    public async Task<T?> GetOrAdd<T>(String key, Func<Task<T?>> fetch, TimeSpan? expiration = null) where T : class?
    {
        if (cache.TryGetValue(key, out T? value))
            return value;

        value = await Run(Tag<T>(key), fetch);

        cache.Set(key, value, CreateCacheEntryOptions(expiration ?? TimeSpan.FromDays(1)));

        return value;
    }

    public Response<T>? Get<T>(String key) where T : class?
    {
        return cache.TryGetValue(key, out Response<T>? response)
            ? response
            : null;
    }

    public T? GetValue<T>(String key) where T : class?
    {
        return cache.TryGetValue(key, out T? value)
            ? value
            : null;
    }

    public void Add<T>(String key, Response<T> value, TimeSpan? expiration = null) where T : class?
    {
        cache.Set(key, value, CreateCacheEntryOptions(expiration ?? TimeSpan.FromDays(1)));
    }

    public void AddValue<T>(String key, T? value, TimeSpan? expiration = null) where T : class?
    {
        cache.Set(key, value, CreateCacheEntryOptions(expiration ?? TimeSpan.FromDays(1)));
    }

    public void Invalidate(String key)
    {
        cache.Remove(key);
    }

    private static MemoryCacheEntryOptions CreateCacheEntryOptions(TimeSpan expiration)
    {
        return new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(expiration)
            .SetSize(1);
    }

    private async Task<T?> Run<T>(String key, Func<Task<T?>> fetch) where T : class?
    {
        var lazy = _gets.GetOrAdd(key, _ =>
            new(async () => (Object?)await fetch(), LazyThreadSafetyMode.ExecutionAndPublication));

        var task = lazy.Value;

        try
        {
            return (T?)await task;
        }
        finally
        {
            if (task.IsCompleted)
                _gets.TryRemove(new(key, lazy));
        }
    }

    private static String Tag<T>(String key)
    {
        return $"{typeof(T).FullName}:{key}";
    }
}