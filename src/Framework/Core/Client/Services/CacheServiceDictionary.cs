using System.Collections.Concurrent;

namespace Crudspa.Framework.Core.Client.Services;

public class CacheServiceDictionary : ICacheService
{
    private readonly ConcurrentDictionary<String, Object> _cache = new();
    private readonly ConcurrentDictionary<String, Lazy<Task<Object?>>> _gets = new(StringComparer.Ordinal);

    public async Task<Response<T>> GetOrAdd<T>(String key, Func<Task<Response<T>>> fetch, TimeSpan? expiration = null) where T : class?
    {
        var response = Get<T>(key);

        if (response is not null)
            return response;

        response = await Run(Tag<Response<T>>(key), async () => (Response<T>?)await fetch()) ?? new();

        if (response.Ok)
            Add(key, response, TimeSpan.Zero);

        return response;
    }

    public async Task<T?> GetOrAdd<T>(String key, Func<Task<T?>> fetch, TimeSpan? expiration = null) where T : class?
    {
        var value = GetValue<T>(key);

        if (value is not null)
            return value;

        value = await Run(Tag<T>(key), fetch);

        AddValue(key, value, TimeSpan.Zero);

        return value;
    }

    public Response<T>? Get<T>(String key) where T : class?
    {
        return _cache.TryGetValue(key, out var value) && value is Response<T> response
            ? response
            : null;
    }

    public T? GetValue<T>(String key) where T : class?
    {
        return _cache.TryGetValue(key, out var value) && value is T typedValue
            ? typedValue
            : null;
    }

    public void Add<T>(String key, Response<T> value, TimeSpan? expiration = null) where T : class?
    {
        _cache[key] = value;
    }

    public void AddValue<T>(String key, T? value, TimeSpan? expiration = null) where T : class?
    {
        _cache[key] = value!;
    }

    public void Invalidate(String key)
    {
        _cache.TryRemove(key, out _);
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