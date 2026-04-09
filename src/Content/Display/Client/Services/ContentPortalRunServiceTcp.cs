namespace Crudspa.Content.Display.Client.Services;

public class ContentPortalRunServiceTcp(IProxyWrappers proxyWrappers) : IContentPortalRunService
{
    public async Task<Response<ContentPortal?>> Fetch(Request<ContentPortal> request) =>
        await proxyWrappers.Send<ContentPortal?>("ContentPortalRunFetch", request);
}