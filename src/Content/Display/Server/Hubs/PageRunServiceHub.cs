using Crudspa.Framework.Core.Server.Contracts;

namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<Page?>> PageRunFetch(Request<Page> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var cacheKey = String.Format(CacheKeys.Page, request.Value.Id);

            var fetchResponse = await CacheService.GetOrAdd(cacheKey, async () => await PageRunService.Fetch(request));

            _ = PageRunService.MarkViewed(request);

            return fetchResponse;
        });
    }

    public async Task<Response> PageRunMarkViewed(Request<Page> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await PageRunService.MarkViewed(request));
    }
}