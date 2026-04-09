namespace Crudspa.Education.Common.Client.Services;

public class ActivityMediaPlayServiceTcp(IProxyWrappers proxyWrappers) : IActivityMediaPlayService
{
    public async Task<Response> Add(Request<ActivityMediaPlay> request) =>
        await proxyWrappers.Send("ActivityMediaPlayAdd", request);
}