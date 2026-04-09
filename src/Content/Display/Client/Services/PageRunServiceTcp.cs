namespace Crudspa.Content.Display.Client.Services;

public class PageRunServiceTcp(IProxyWrappers proxyWrappers) : IPageRunService
{
    public async Task<Response<Page?>> Fetch(Request<Page> request) =>
        await proxyWrappers.Send<Page?>("PageRunFetch", request);

    public async Task<Response> MarkViewed(Request<Page> request) =>
        await proxyWrappers.Send("PageRunMarkViewed", request);
}