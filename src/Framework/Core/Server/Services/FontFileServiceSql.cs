namespace Crudspa.Framework.Core.Server.Services;

public class FontFileServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IBlobService blobService)
    : IFontFileService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<FontFile?>> Fetch(Request<FontFile> request)
    {
        return await wrappers.Try<FontFile?>(request, async response =>
            await FontFileSelect.Execute(Connection, request.Value));
    }

    public async Task<Response<FontFile?>> Add(Request<FontFile> request)
    {
        return await wrappers.Try<FontFile?>(request, async response =>
        {
            var fontFile = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                fontFile.Id = await FontFileInsert.Execute(connection, transaction, request.SessionId, fontFile));

            return await FontFileSelect.Execute(Connection, fontFile);
        });
    }

    public async Task<Response> Save(Request<FontFile> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var fontFile = request.Value;

            var existingFontFile = await FontFileSelect.Execute(Connection, fontFile);

            if (fontFile.BlobId != existingFontFile!.BlobId)
                await blobService.Remove(new Blob { Id = existingFontFile.BlobId });

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await FontFileUpdate.Execute(connection, transaction, request.SessionId, fontFile));
        });
    }

    public async Task<Response> Remove(Request<FontFile> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var fontFile = request.Value;

            var existingFontFile = await FontFileSelect.Execute(Connection, fontFile);

            await blobService.Remove(new Blob { Id = existingFontFile!.BlobId });

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await FontFileDelete.Execute(connection, transaction, request.SessionId, fontFile));
        });
    }
}