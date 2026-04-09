namespace Crudspa.Framework.Core.Client.Services;

public class PortalServiceTcp(IProxyWrappers proxyWrappers) : IPortalService
{
    public async Task<Response<IList<Portal>>> FetchAll(Request request) =>
        await proxyWrappers.Send<IList<Portal>>("PortalFetchAll", request);

    public async Task<Response<Portal?>> Fetch(Request<Portal> request) =>
        await proxyWrappers.Send<Portal?>("PortalFetch", request);

    public async Task<Response> Save(Request<Portal> request) =>
        await proxyWrappers.Send("PortalSave", request);
}