namespace Crudspa.Content.Design.Client.Services;

public class BlogServiceTcp(IProxyWrappers proxyWrappers) : IBlogService
{
    public async Task<Response<IList<Blog>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Blog>>("BlogFetchForPortal", request);

    public async Task<Response<Blog?>> Fetch(Request<Blog> request) =>
        await proxyWrappers.Send<Blog?>("BlogFetch", request);

    public async Task<Response<Blog?>> Add(Request<Blog> request) =>
        await proxyWrappers.Send<Blog?>("BlogAdd", request);

    public async Task<Response> Save(Request<Blog> request) =>
        await proxyWrappers.Send("BlogSave", request);

    public async Task<Response> Remove(Request<Blog> request) =>
        await proxyWrappers.Send("BlogRemove", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("BlogFetchContentStatusNames", request);
}