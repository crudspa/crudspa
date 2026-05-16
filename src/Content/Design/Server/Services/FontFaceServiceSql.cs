namespace Crudspa.Content.Design.Server.Services;

public class FontFaceServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IBlobService blobService)
    : IFontFaceService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<FontFace>>> FetchForFont(Request<Font> request)
    {
        return await wrappers.Try<IList<FontFace>>(request, async response =>
            await FontFaceSelectForFont.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<FontFace?>> Fetch(Request<FontFace> request)
    {
        return await wrappers.Try<FontFace?>(request, async response =>
            await FontFaceSelect.Execute(Connection, request.SessionId, request.Value));
    }

    public async Task<Response<FontFace?>> Add(Request<FontFace> request)
    {
        return await wrappers.Try<FontFace?>(request, async response =>
        {
            var fontFace = request.Value;

            await ApplyFontMetadata(fontFace, null, response);
            response.AddErrors(fontFace.Validate());
            if (response.Errors.HasItems())
                return null;

            var fileFileResponse = await fileService.SaveFont(new(request.SessionId, fontFace.FileFile));
            if (!fileFileResponse.Ok)
            {
                response.AddErrors(fileFileResponse.Errors);
                return null;
            }

            fontFace.FileFile.Id = fileFileResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await FontFaceInsert.Execute(connection, transaction, request.SessionId, fontFace);

                return new FontFace
                {
                    Id = id,
                    FontId = fontFace.FontId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<FontFace> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var fontFace = request.Value;
            var existing = await FontFaceSelect.Execute(Connection, request.SessionId, fontFace);

            await ApplyFontMetadata(fontFace, existing, response);
            response.AddErrors(fontFace.Validate());
            if (response.Errors.HasItems())
                return;

            var fileFileResponse = await fileService.SaveFont(new(request.SessionId, fontFace.FileFile), existing?.FileFile);
            if (!fileFileResponse.Ok)
            {
                response.AddErrors(fileFileResponse.Errors);
                return;
            }

            fontFace.FileFile.Id = fileFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await FontFaceUpdate.Execute(connection, transaction, request.SessionId, fontFace));
        });
    }

    public async Task<Response> Remove(Request<FontFace> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var fontFace = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await FontFaceDelete.Execute(connection, transaction, request.SessionId, fontFace));
        });
    }

    private async Task ApplyFontMetadata(FontFace fontFace, FontFace? existing, Response response)
    {
        var blobId = fontFace.FileFile.BlobId ?? existing?.FileFile.BlobId;
        if (!blobId.HasValue)
            return;

        var stream = await blobService.TryFetchStream(blobId.Value);
        if (stream is null)
        {
            response.AddError("Font file could not be found.", nameof(FontFace.FileFile));
            return;
        }

        await using (stream)
        {
            var metadata = await FontMetadataReader.Read(stream);
            if (metadata is null)
            {
                response.AddError("Font metadata could not be read. Upload a valid TTF, OTF, WOFF, or WOFF2 font file.", nameof(FontFace.FileFile));
                return;
            }

            fontFace.Style = metadata.Style;
            fontFace.WeightMin = metadata.WeightMin;
            fontFace.WeightMax = metadata.WeightMax;
        }
    }
}