namespace Crudspa.Framework.Core.Client.Services;

public class PortalRunServiceTcp(IProxyWrappers proxyWrappers) : IPortalRunService
{
    public async Task<Response<Portal?>> Fetch(Request<Portal> request) =>
        await proxyWrappers.Send<Portal?>("PortalRunFetch", request);
}