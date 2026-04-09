namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<ActivityAssignment>> ActivityRunAddActivityState(Request<ActivityAssignment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await ActivityRunService.AddActivityState(request));
    }

    public async Task<Response> ActivityRunSaveActivityState(Request<ActivityAssignment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await ActivityRunService.SaveActivityState(request));
    }
}