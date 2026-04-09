namespace Crudspa.Education.Publisher.Client.Services;

public class ActivityElementServiceTcp(IProxyWrappers proxyWrappers) : IActivityElementService
{
    public async Task<Response<IList<ActivityTypeFull>>> FetchActivityTypes(Request request) =>
        await proxyWrappers.SendAndCache<IList<ActivityTypeFull>>("ActivityElementFetchActivityTypes", request);

    public async Task<Response<IList<Named>>> FetchContentAreaNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Named>>("ActivityElementFetchContentAreaNames", request);
}