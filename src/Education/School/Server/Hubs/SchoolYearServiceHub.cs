namespace Crudspa.Education.School.Server.Hubs;

public partial class SchoolHub
{
    public async Task<Response<SchoolYear?>> SchoolYearFetchCurrent(Request request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await SchoolYearService.FetchCurrent(request));
    }
}