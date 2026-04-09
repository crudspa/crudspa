using Crudspa.Framework.Core.Server.Contracts;

namespace Crudspa.Content.Display.Server.Services;

public class ContentPortalRunServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    IPageRunService pageRunService,
    ICacheService cacheService)
    : IContentPortalRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<ContentPortal?>> Fetch(Request<ContentPortal> request)
    {
        return await wrappers.Try<ContentPortal?>(request, async response =>
        {
            var portalId = configService.Fetch().PortalId;
            var cacheKey = String.Format(CacheKeys.Portal, portalId);

            var fetchResponse = await cacheService.GetOrAdd<ContentPortal>(cacheKey, async () =>
            {
                var portal = await ContentPortalSelect.Execute(Connection, portalId);

                return new(portal);
            });

            if (!fetchResponse.Ok || fetchResponse.Value is null)
                return null;

            var portal = fetchResponse.Value;

            portal.FooterPage = null;

            if (portal.FooterPageId is not null)
            {
                var page = new Page { Id = portal.FooterPageId };

                var footerResponse = await pageRunService.Fetch(new(request.SessionId, page));

                if (footerResponse.Ok)
                    portal.FooterPage = footerResponse.Value;
            }

            return portal;
        });
    }
}