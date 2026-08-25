namespace Crudspa.Content.Design.Server.Services;

public class ForumServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAudioFileService audioFileService,
    IImageFileService imageFileService,
    IPdfFileService pdfFileService,
    IVideoFileService videoFileService,
    IBlobService blobService,
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

            if (!await ForumPortalAuthorize.Execute(Connection, request.SessionId, forum.PortalId))
            {
                response.AddError("Portal was not found or is not available for forum authoring.", nameof(Forum.PortalId));
                return null;
            }

            if (forum.ImageFile.Id.HasValue || forum.ImageFile.BlobId.HasValue)
            {
                var coverResponse = await FetchNewCover(request.SessionId, forum.PortalId, null, forum.ImageFile);
                if (!coverResponse.Ok)
                {
                    response.AddErrors(coverResponse.Errors);
                    return null;
                }

                forum.ImageFile = coverResponse.Value;
            }
            else
                forum.ImageFile = new();

            forum.Description = htmlSanitizer.Sanitize(forum.Description);

            try
            {
                return await sqlWrappers.WithConnection(async (connection, transaction) =>
                {
                    var id = await ForumInsert.Execute(connection, transaction, request.SessionId, forum);

                    return new Forum
                    {
                        Id = id,
                        PortalId = forum.PortalId,
                    };
                });
            }
            catch
            {
                if (forum.ImageFile.Id is { } imageId)
                    await RemoveFile(request.SessionId, new(CommentMedia.Types.Image, imageId));

                throw;
            }
        });
    }

    public async Task<Response> Save(Request<Forum> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var forum = request.Value;

            var existing = await ForumSelect.Execute(Connection, request.SessionId, forum);

            if (existing is null)
            {
                response.AddError("Forum was not found.");
                return;
            }

            var removingImage = !forum.ImageFile.Id.HasValue && !forum.ImageFile.BlobId.HasValue;
            var unchangedImage = forum.ImageFile.Id == existing.ImageFile.Id
                                 && forum.ImageFile.BlobId == existing.ImageFile.BlobId;
            var replacingImage = !unchangedImage;

            if (unchangedImage)
                forum.ImageFile = existing.ImageFile;
            else if (removingImage)
                forum.ImageFile = new();
            else
            {
                var coverResponse = await FetchNewCover(request.SessionId, existing.PortalId, forum.Id, forum.ImageFile);
                if (!coverResponse.Ok)
                {
                    response.AddErrors(coverResponse.Errors);
                    return;
                }

                forum.ImageFile = coverResponse.Value;
            }

            forum.Description = htmlSanitizer.Sanitize(forum.Description);

            try
            {
                await sqlWrappers.WithConnection(async (connection, transaction) =>
                {
                    await ForumUpdate.Execute(connection, transaction, request.SessionId, forum);
                });
            }
            catch
            {
                if (replacingImage && forum.ImageFile.Id is { } imageId)
                    await RemoveFile(request.SessionId, new(CommentMedia.Types.Image, imageId));

                throw;
            }

            if (replacingImage
                && existing.ImageFile.Id is { } existingImageId
                && !await ForumRunCommentMediaFileReference.Exists(Connection, CommentMedia.Types.Image,
                    existingImageId))
                await RemoveImageFile(request.SessionId, existingImageId);
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

            var files = await ForumDeleteFileSelect.Execute(Connection, request.SessionId, forum.Id);
            var filesToRemove = files.CommentMediaFiles.ToList();
            if (files.CoverImageId.HasValue)
                filesToRemove.Add(new(CommentMedia.Types.Image, files.CoverImageId));

            foreach (var file in filesToRemove.Where(x => x.Id.HasValue).DistinctBy(x => (x.Type, x.Id)))
            {
                if (await ForumRunCommentMediaFileReference.Exists(Connection, file.Type, file.Id!.Value))
                    continue;

                await RemoveFile(request.SessionId, file);
            }
        });
    }

    private async Task RemoveFile(Guid? sessionId, ForumDeleteFileSelect.File file)
    {
        if (!file.Id.HasValue
            || await ForumRunCommentMediaFileReference.Exists(Connection, file.Type, file.Id.Value))
            return;

        switch (file.Type)
        {
            case CommentMedia.Types.Audio:
                await audioFileService.Remove(new(sessionId, new AudioFile { Id = file.Id }));
                break;
            case CommentMedia.Types.Image:
                await RemoveImageFile(sessionId, file.Id);
                break;
            case CommentMedia.Types.Pdf:
                await pdfFileService.Remove(new(sessionId, new PdfFile { Id = file.Id }));
                break;
            case CommentMedia.Types.Video:
                await videoFileService.Remove(new(sessionId, new VideoFile { Id = file.Id }));
                break;
        }
    }

    private async Task RemoveImageFile(Guid? sessionId, Guid? id)
    {
        var fetchResponse = await imageFileService.Fetch(new(sessionId, new() { Id = id }));
        if (!fetchResponse.Ok || fetchResponse.Value is null)
            return;

        var image = fetchResponse.Value;
        var removeResponse = await imageFileService.Remove(new(sessionId, image));
        if (!removeResponse.Ok)
            return;

        var derivedBlobIds = new[]
            {
                image.OptimizedBlobId,
                image.Resized96BlobId,
                image.Resized192BlobId,
                image.Resized360BlobId,
                image.Resized540BlobId,
                image.Resized720BlobId,
                image.Resized1080BlobId,
                image.Resized1440BlobId,
                image.Resized1920BlobId,
                image.Resized3840BlobId,
            }
            .Where(x => x.HasValue && x != image.BlobId)
            .Distinct();

        foreach (var blobId in derivedBlobIds)
            await blobService.Remove(new Blob { Id = blobId });
    }

    private static Boolean IsValidSavedCover(ImageFile imageFile)
    {
        if (!imageFile.BlobId.HasValue)
            return true;

        return ForumMediaPolicy.HasValidImageDimensions(imageFile.Width, imageFile.Height)
               && ForumMediaPolicy.ImageExtensions.HasAny(x => x.IsBasically(imageFile.Format));
    }

    private async Task<Response<ImageFile?>> FetchNewCover(Guid? sessionId, Guid? portalId, Guid? forumId,
        ImageFile requestedImage)
    {
        if (!requestedImage.Id.HasValue || !requestedImage.BlobId.HasValue
            || !await ForumCoverFileAuthorize.Execute(Connection, sessionId, portalId, forumId,
                requestedImage.Id, requestedImage.BlobId))
            return new("Forum cover must be a new image uploaded for this forum edit session.");

        if (await ForumRunCommentMediaFileReference.Exists(Connection, CommentMedia.Types.Image,
                requestedImage.Id.Value))
            return new("Forum cover image is already used by other content.");

        var fetchResponse = await imageFileService.Fetch(new(sessionId, new() { Id = requestedImage.Id }));
        var image = fetchResponse.Value;

        if (!fetchResponse.Ok || image?.BlobId != requestedImage.BlobId
            || !IsValidSavedCover(image)
            || !await IsCoverBlobWithinLimit(image.BlobId))
        {
            if (image?.Id.HasValue == true)
                await RemoveImageFile(sessionId, image.Id);

            return new("Forum cover is missing, invalid, or exceeds the 10 MiB limit.");
        }

        return new(image);
    }

    private async Task<Boolean> IsCoverBlobWithinLimit(Guid? blobId)
    {
        if (!blobId.HasValue)
            return false;

        await using var stream = await blobService.TryFetchStream(blobId.Value);
        if (stream is null)
            return false;

        if (stream.CanSeek)
            return stream.Length is > 0 and <= ForumMediaPolicy.ImageMaxBytes;

        var buffer = new Byte[81920];
        Int64 total = 0;

        while (total <= ForumMediaPolicy.ImageMaxBytes)
        {
            var read = await stream.ReadAsync(buffer);
            if (read == 0)
                return total > 0;

            total += read;
        }

        return false;
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

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request<Portal> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await ForumPermissionSelectNames.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<IList<Named>>> FetchLicenseNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await LicenseSelectNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Named>>> FetchBundleNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await BundleSelectNames.Execute(Connection));
    }
}