namespace Crudspa.Content.Design.Server.Services;

public class ForumServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IForumService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Forum>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Forum>>(request, async response =>
        {
            var forums = await ForumSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);
            return forums;
        });
    }

    public async Task<Response<Forum?>> Fetch(Request<Forum> request)
    {
        return await wrappers.Try<Forum?>(request, async response =>
        {
            var forum = await ForumSelect.Execute(Connection, request.SessionId, request.Value);
            return forum;
        });
    }

    public async Task<Response<Forum?>> Add(Request<Forum> request)
    {
        return await wrappers.Validate<Forum?, Forum>(request, async response =>
        {
            var forum = request.Value;

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, forum.ImageFile));
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return null;
            }

            forum.ImageFile.Id = imageFileResponse.Value.Id;

            forum.Description = htmlSanitizer.Sanitize(forum.Description);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ForumInsert.Execute(connection, transaction, request.SessionId, forum);

                return new Forum
                {
                    Id = id,
                    PortalId = forum.PortalId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Forum> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var forum = request.Value;

            var existing = await ForumSelect.Execute(Connection, request.SessionId, forum);

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, forum.ImageFile), existing?.ImageFile);
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return;
            }

            forum.ImageFile.Id = imageFileResponse.Value.Id;

            forum.Description = htmlSanitizer.Sanitize(forum.Description);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ForumUpdate.Execute(connection, transaction, request.SessionId, forum);
            });
        });
    }

    public async Task<Response> Remove(Request<Forum> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var forum = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ForumDelete.Execute(connection, transaction, request.SessionId, forum);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Forum>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var forums = request.Value;

            forums.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ForumUpdateOrdinals.Execute(connection, transaction, request.SessionId, forums);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }
}