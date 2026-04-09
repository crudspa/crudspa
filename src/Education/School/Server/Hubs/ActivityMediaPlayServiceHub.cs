namespace Crudspa.Education.School.Server.Hubs;

public partial class SchoolHub
{
    public async Task<Response> ActivityMediaPlayAdd(Request<ActivityMediaPlay> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await ActivityMediaPlayService.Add(request));
    }
}