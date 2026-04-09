namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<ActivityTypeFull>>> ActivityElementFetchActivityTypes(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ActivityElementService.FetchActivityTypes(request));
    }

    public async Task<Response<IList<Named>>> ActivityElementFetchContentAreaNames(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ActivityElementService.FetchContentAreaNames(request));
    }
}