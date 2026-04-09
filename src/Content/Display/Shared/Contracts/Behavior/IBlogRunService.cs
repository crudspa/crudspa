namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface IBlogRunService
{
    Task<Response<IList<Blog>>> FetchBlogs(Request request);
    Task<Response<Blog?>> FetchBlog(Request<Blog> request);
    Task<Response<Post?>> FetchPost(Request<Post> request);
}