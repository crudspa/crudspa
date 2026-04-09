namespace Crudspa.Education.Common.Client.Services;

public class ActivityRunServiceTcp(IProxyWrappers proxyWrappers) : IActivityRunService
{
    public async Task<Response<ActivityAssignment>> AddActivityState(Request<ActivityAssignment> request) =>
        await proxyWrappers.Send<ActivityAssignment>("ActivityRunAddActivityState", request);

    public async Task<Response> SaveActivityState(Request<ActivityAssignment> request) =>
        await proxyWrappers.Send("ActivityRunSaveActivityState", request);
}