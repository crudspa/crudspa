using Crudspa.Framework.Core.Server.Contracts;

namespace Crudspa.Framework.Core.Server.Services;

public class SessionFetcherCache(IServerConfigService configService, ICacheService cacheService) : ISessionFetcher
{
    private static readonly TimeSpan SessionCacheExpiration = TimeSpan.FromMinutes(30);
    private readonly SessionFetcherSql _sessionFetcherSql = new(configService);
    private Int64 _cacheVersion = 1;

    public async Task<Session?> Fetch(Guid? sessionId)
    {
        if (sessionId is not { } validSessionId)
            return null;

        var cacheKey = BuildCacheKey(validSessionId);
        var cached = cacheService.GetValue<Session>(cacheKey);

        if (cached is not null)
            return cached;

        var fetched = await _sessionFetcherSql.Fetch(validSessionId);

        if (fetched is not null)
            cacheService.AddValue(cacheKey, fetched, SessionCacheExpiration);

        return fetched;
    }

    public void Invalidate(Guid? sessionId)
    {
        if (sessionId is not { } validSessionId)
            return;

        var cacheKey = BuildCacheKey(validSessionId);

        cacheService.Invalidate(cacheKey);
    }

    public void InvalidateAll()
    {
        Interlocked.Increment(ref _cacheVersion);
    }

    private String BuildCacheKey(Guid sessionId)
    {
        var version = Interlocked.Read(ref _cacheVersion);
        return $"{String.Format(CacheKeys.Session, sessionId)}-V{version:D}";
    }
}