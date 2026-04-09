namespace Crudspa.Education.Student.Server.Hubs;

public partial class StudentHub
{
    public async Task<Response> ActivityMediaPlayAdd(Request<ActivityMediaPlay> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await ActivityMediaPlayService.Add(request));
    }
}