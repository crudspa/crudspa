namespace Crudspa.Content.Design.Client.Services;

public class ForumServiceTcp(IProxyWrappers proxyWrappers) : IForumService
{
    public async Task<Response<IList<Forum>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Forum>>("ForumFetchForPortal", request);

    public async Task<Response<Forum?>> Fetch(Request<Forum> request) =>
        await proxyWrappers.Send<Forum?>("ForumFetch", request);

    public async Task<Response<Forum?>> Add(Request<Forum> request) =>
        await proxyWrappers.Send<Forum?>("ForumAdd", request);

    public async Task<Response> Save(Request<Forum> request) =>
        await proxyWrappers.Send("ForumSave", request);

    public async Task<Response> Remove(Request<Forum> request) =>
        await proxyWrappers.Send("ForumRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Forum>> request) =>
        await proxyWrappers.Send("ForumSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ForumFetchContentStatusNames", request);
}