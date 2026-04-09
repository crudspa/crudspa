namespace Crudspa.Content.Display.Client.Services;

public class BlogRunServiceTcp(IProxyWrappers proxyWrappers) : IBlogRunService
{
    public async Task<Response<IList<Blog>>> FetchBlogs(Request request) =>
        await proxyWrappers.Send<IList<Blog>>("BlogRunFetchBlogs", request);

    public async Task<Response<Blog?>> FetchBlog(Request<Blog> request) =>
        await proxyWrappers.Send<Blog?>("BlogRunFetchBlog", request);

    public async Task<Response<Post?>> FetchPost(Request<Post> request) =>
        await proxyWrappers.Send<Post?>("BlogRunFetchPost", request);
}