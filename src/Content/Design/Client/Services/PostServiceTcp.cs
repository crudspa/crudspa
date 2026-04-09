namespace Crudspa.Content.Design.Client.Services;

public class PostServiceTcp(IProxyWrappers proxyWrappers) : IPostService
{
    public async Task<Response<IList<Post>>> SearchForBlog(Request<PostSearch> request) =>
        await proxyWrappers.Send<IList<Post>>("PostSearchForBlog", request);

    public async Task<Response<Post?>> Fetch(Request<Post> request) =>
        await proxyWrappers.Send<Post?>("PostFetch", request);

    public async Task<Response<Post?>> FetchPageId(Request<Post> request) =>
        await proxyWrappers.Send<Post?>("PostFetchPageId", request);

    public async Task<Response<Post?>> Add(Request<Post> request) =>
        await proxyWrappers.Send<Post?>("PostAdd", request);

    public async Task<Response> Save(Request<Post> request) =>
        await proxyWrappers.Send("PostSave", request);

    public async Task<Response> Remove(Request<Post> request) =>
        await proxyWrappers.Send("PostRemove", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("PostFetchContentStatusNames", request);

    public async Task<Response<IList<Orderable>>> FetchPageTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("PostFetchPageTypeNames", request);

    public async Task<Response<Page?>> FetchPage(Request<PostPage> request) =>
        await proxyWrappers.Send<Page?>("PostFetchPage", request);

    public async Task<Response> SavePage(Request<PostPage> request) =>
        await proxyWrappers.Send("PostSavePage", request);

    public async Task<Response<IList<Section>>> FetchSections(Request<PostSection> request) =>
        await proxyWrappers.Send<IList<Section>>("PostFetchSections", request);

    public async Task<Response<Section?>> FetchSection(Request<PostSection> request) =>
        await proxyWrappers.Send<Section?>("PostFetchSection", request);

    public async Task<Response<Section?>> AddSection(Request<PostSection> request) =>
        await proxyWrappers.Send<Section?>("PostAddSection", request);

    public async Task<Response> SaveSection(Request<PostSection> request) =>
        await proxyWrappers.Send("PostSaveSection", request);

    public async Task<Response> RemoveSection(Request<PostSection> request) =>
        await proxyWrappers.Send("PostRemoveSection", request);

    public async Task<Response> SaveSectionOrder(Request<PostSection> request) =>
        await proxyWrappers.Send("PostSaveSectionOrder", request);
}