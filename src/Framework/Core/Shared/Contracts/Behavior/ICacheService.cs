namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface ICacheService
{
    Task<Response<T>> GetOrAdd<T>(String key, Func<Task<Response<T>>> fetchFunction, TimeSpan? expiration = null) where T : class?;
    Task<T?> GetOrAdd<T>(String key, Func<Task<T?>> fetchFunction, TimeSpan? expiration = null) where T : class?;
    Response<T>? Get<T>(String key) where T : class?;
    T? GetValue<T>(String key) where T : class?;
    void Add<T>(String key, Response<T> value, TimeSpan? expiration = null) where T : class?;
    void AddValue<T>(String key, T? value, TimeSpan? expiration = null) where T : class?;
    void Invalidate(String key);
}