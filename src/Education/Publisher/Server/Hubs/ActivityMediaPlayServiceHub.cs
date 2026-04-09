namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response> ActivityMediaPlayAdd(Request<ActivityMediaPlay> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await ActivityMediaPlayService.Add(request));
    }
}