namespace Crudspa.Framework.Core.Client.Services;

public class SessionStateServiceTcp(IProxyWrappers proxyWrappers) : ISessionStateService
{
    public async Task<Response<Session?>> Fetch(Request<Session> request) =>
        await proxyWrappers.Send<Session?>("SessionStateFetch", request);
}