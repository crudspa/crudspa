namespace Crudspa.Framework.Core.Server.Services;

public class ImageFileServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IBlobService blobService)
    : IImageFileService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<ImageFile?>> Fetch(Request<ImageFile> request)
    {
        return await wrappers.Try<ImageFile?>(request, async response =>
            await ImageFileSelect.Execute(Connection, request.Value));
    }

    public async Task<Response<ImageFile?>> Add(Request<ImageFile> request)
    {
        return await wrappers.Validate<ImageFile?, ImageFile>(request, async response =>
        {
            var imageFile = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                imageFile.Id = await ImageFileInsert.Execute(connection, transaction, request.SessionId, imageFile));

            return await ImageFileSelect.Execute(Connection, imageFile);
        });
    }

    public async Task<Response> Save(Request<ImageFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var imageFile = request.Value;

            var existing = await ImageFileSelect.Execute(Connection, imageFile);

            if (existing is not null)
            {
                if (imageFile.BlobId != existing.BlobId)
                {
                    await blobService.Remove(new Blob { Id = existing.BlobId });
                    imageFile.Caption = null;
                }
                else
                    imageFile.Caption = existing.Caption;
            }
            else
                imageFile.Caption = null;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await ImageFileUpdate.Execute(connection, transaction, request.SessionId, imageFile));
        });
    }

    public async Task<Response> Remove(Request<ImageFile> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var imageFile = request.Value;

            var existingImageFile = await ImageFileSelect.Execute(Connection, imageFile);

            if (existingImageFile is not null)
            {
                await blobService.Remove(new Blob { Id = existingImageFile.BlobId });

                await sqlWrappers.WithConnection(async (connection, transaction) =>
                    await ImageFileDelete.Execute(connection, transaction, request.SessionId, imageFile));
            }
        });
    }
}