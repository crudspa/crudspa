namespace Crudspa.Education.District.Server.Hubs;

public partial class DistrictHub
{
    public async Task<Response> ActivityMediaPlayAdd(Request<ActivityMediaPlay> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await ActivityMediaPlayService.Add(request));
    }
}