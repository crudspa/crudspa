namespace Crudspa.Framework.Core.Shared.Services;

public class CacheServiceNull : ICacheService
{
    public Task<Response<T>> GetOrAdd<T>(String key, Func<Task<Response<T>>> fetchFunction, TimeSpan? expiration = null) where T : class? => fetchFunction();
    public Task<T?> GetOrAdd<T>(String key, Func<Task<T?>> fetchFunction, TimeSpan? expiration = null) where T : class? => fetchFunction();
    public Response<T>? Get<T>(String key) where T : class? => null;
    public T? GetValue<T>(String key) where T : class? => null;
    public void Add<T>(String key, Response<T> value, TimeSpan? expiration = null) where T : class? { }
    public void AddValue<T>(String key, T? value, TimeSpan? expiration = null) where T : class? { }
    public void Invalidate(String key) { }
}