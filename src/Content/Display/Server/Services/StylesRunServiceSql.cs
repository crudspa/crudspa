using Crudspa.Content.Display.Server.Contracts;

namespace Crudspa.Content.Display.Server.Services;

public class StylesRunServiceSql(
    IServerConfigService configService,
    ICacheService cacheService)
    : IStylesRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<PortalStyles?> FetchForPortal(Guid portalId)
    {
        return await StylesSelectForPortal.Execute(Connection, portalId);
    }

    public async Task<Int32?> FetchStyleRevision(Guid portalId)
    {
        var cacheKey = StylesKeys.Revision(portalId);
        var cached = await cacheService.GetOrAdd(cacheKey, async () =>
        {
            var revision = await StyleRevisionSelectForPortal.Execute(Connection, portalId);

            return revision.HasValue
                ? new StyleRevisionValue { Revision = revision.Value }
                : null;
        });

        return cached?.Revision;
    }

    private sealed class StyleRevisionValue
    {
        public Int32 Revision { get; init; }
    }
}