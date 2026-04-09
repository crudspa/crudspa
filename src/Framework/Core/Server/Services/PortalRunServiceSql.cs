using Crudspa.Framework.Core.Server.Contracts;

namespace Crudspa.Framework.Core.Server.Services;

public class PortalRunServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    ICacheService cacheService)
    : IPortalRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<Portal?>> Fetch(Request<Portal> request)
    {
        return await wrappers.Try<Portal?>(request, async response =>
        {
            var portalId = configService.Fetch().PortalId;
            var cacheKey = String.Format(CacheKeys.Portal, portalId);

            var fetchResponse = await cacheService.GetOrAdd<Portal>(cacheKey, async () =>
            {
                var portal = await PortalRunSelect.Execute(Connection, portalId);

                if (portal is null)
                    throw new($"Portal '{portalId:D}' not found.");

                return new(portal);
            });

            return fetchResponse.Value;
        });
    }
}