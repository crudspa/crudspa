namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<IList<Blog>>> BlogRunFetchBlogs(Request request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await BlogRunService.FetchBlogs(request));
    }

    public async Task<Response<Blog?>> BlogRunFetchBlog(Request<Blog> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await BlogRunService.FetchBlog(request));
    }

    public async Task<Response<Post?>> BlogRunFetchPost(Request<Post> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await BlogRunService.FetchPost(request));
    }
}