namespace Crudspa.Content.Design.Server.Services;

public class FontServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService)
    : IFontService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Font>>> FetchForContentPortal(Request<ContentPortal> request)
    {
        return await wrappers.Try<IList<Font>>(request, async response =>
        {
            var fonts = await FontSelectForContentPortal.Execute(Connection, request.SessionId, request.Value.Id);
            return fonts;
        });
    }

    public async Task<Response<Font?>> Fetch(Request<Font> request)
    {
        return await wrappers.Try<Font?>(request, async response =>
        {
            var font = await FontSelect.Execute(Connection, request.SessionId, request.Value);
            return font;
        });
    }

    public async Task<Response<Font?>> Add(Request<Font> request)
    {
        return await wrappers.Validate<Font?, Font>(request, async response =>
        {
            var font = request.Value;

            var fileFileResponse = await fileService.SaveFont(new(request.SessionId, font.FileFile));
            if (!fileFileResponse.Ok)
            {
                response.AddErrors(fileFileResponse.Errors);
                return null;
            }

            font.FileFile.Id = fileFileResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await FontInsert.Execute(connection, transaction, request.SessionId, font);

                return new Font
                {
                    Id = id,
                    ContentPortalId = font.ContentPortalId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Font> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var font = request.Value;

            var existing = await FontSelect.Execute(Connection, request.SessionId, font);

            var fileFileResponse = await fileService.SaveFont(new(request.SessionId, font.FileFile), existing?.FileFile);
            if (!fileFileResponse.Ok)
            {
                response.AddErrors(fileFileResponse.Errors);
                return;
            }

            font.FileFile.Id = fileFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await FontUpdate.Execute(connection, transaction, request.SessionId, font);
            });
        });
    }

    public async Task<Response> Remove(Request<Font> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var font = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await FontDelete.Execute(connection, transaction, request.SessionId, font);
            });
        });
    }

    public async Task<Response<IList<IconFull>>> FetchIcons(Request request)
    {
        return await wrappers.Try<IList<IconFull>>(request, async response =>
            await IconSelectFull.Execute(Connection));
    }
}