namespace Crudspa.Content.Design.Server.Services;

public class BlogServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IBlogService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Blog>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Blog>>(request, async response =>
        {
            var blogs = await BlogSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);

            return blogs;
        });
    }

    public async Task<Response<Blog?>> Fetch(Request<Blog> request)
    {
        return await wrappers.Try<Blog?>(request, async response =>
        {
            var blog = await BlogSelect.Execute(Connection, request.SessionId, request.Value);

            return blog;
        });
    }

    public async Task<Response<Blog?>> Add(Request<Blog> request)
    {
        return await wrappers.Validate<Blog?, Blog>(request, async response =>
        {
            var blog = request.Value;

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, blog.ImageFile));
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return null;
            }

            blog.ImageFile.Id = imageFileResponse.Value.Id;

            blog.Description = htmlSanitizer.Sanitize(blog.Description);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await BlogInsert.Execute(connection, transaction, request.SessionId, blog);

                return new Blog
                {
                    Id = id,
                    PortalId = blog.PortalId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Blog> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var blog = request.Value;

            var existing = await BlogSelect.Execute(Connection, request.SessionId, blog);

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, blog.ImageFile), existing?.ImageFile);
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return;
            }

            blog.ImageFile.Id = imageFileResponse.Value.Id;

            blog.Description = htmlSanitizer.Sanitize(blog.Description);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await BlogUpdate.Execute(connection, transaction, request.SessionId, blog);
            });
        });
    }

    public async Task<Response> Remove(Request<Blog> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var blog = request.Value;
            var existing = await BlogSelect.Execute(Connection, request.SessionId, blog);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await BlogDelete.Execute(connection, transaction, request.SessionId, blog);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }
}