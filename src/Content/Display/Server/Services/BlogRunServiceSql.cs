namespace Crudspa.Content.Display.Server.Services;

public class BlogRunServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    ISessionLicenseResolver sessionLicenseResolver)
    : IBlogRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Blog>>> FetchBlogs(Request request)
    {
        return await wrappers.Try<IList<Blog>>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await BlogSelectAll.Execute(Connection, request.SessionId, licenseIds);
        });
    }

    public async Task<Response<Blog?>> FetchBlog(Request<Blog> request)
    {
        return await wrappers.Try<Blog?>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await BlogSelectRun.Execute(Connection, request.Value.Id, request.SessionId, licenseIds);
        });
    }

    public async Task<Response<Post?>> FetchPost(Request<Post> request)
    {
        return await wrappers.Try<Post?>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await PostSelectRun.Execute(Connection, request.Value, request.SessionId, licenseIds);
        });
    }
}