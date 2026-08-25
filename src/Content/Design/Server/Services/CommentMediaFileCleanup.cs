namespace Crudspa.Content.Design.Server.Services;

internal class CommentMediaFileCleanup(
    String connection,
    IAudioFileService audioFileService,
    IImageFileService imageFileService,
    IPdfFileService pdfFileService,
    IVideoFileService videoFileService,
    IBlobService blobService)
{
    public static Guid? FileId(CommentMedia media) => media.Type switch
    {
        CommentMedia.Types.Audio => media.AudioFile.Id,
        CommentMedia.Types.Image => media.ImageFile.Id,
        CommentMedia.Types.Pdf => media.PdfFile.Id,
        CommentMedia.Types.Video => media.VideoFile.Id,
        _ => null,
    };

    public static Guid? BlobId(CommentMedia media) => media.Type switch
    {
        CommentMedia.Types.Audio => media.AudioFile.BlobId,
        CommentMedia.Types.Image => media.ImageFile.BlobId,
        CommentMedia.Types.Pdf => media.PdfFile.BlobId,
        CommentMedia.Types.Video => media.VideoFile.BlobId,
        _ => null,
    };

    public static IList<CommentMedia> Difference(IEnumerable<CommentMedia> source,
        IEnumerable<CommentMedia> other)
    {
        var compared = other.ToList();
        return source.Where(media => compared.All(item => !SameFile(media, item))).ToList();
    }

    public static Boolean SameFile(CommentMedia left, CommentMedia right) =>
        left.Type == right.Type
        && FileId(left) == FileId(right)
        && BlobId(left) == BlobId(right);

    public async Task Cleanup(Guid? sessionId, IEnumerable<CommentMedia> medias)
    {
        foreach (var media in medias
                     .Where(x => FileId(x).HasValue || BlobId(x).HasValue)
                     .DistinctBy(x => (x.Type, FileId(x), BlobId(x))))
        {
            var fileId = FileId(media);
            if (fileId.HasValue)
            {
                if (await ForumRunCommentMediaFileReference.Exists(connection, media.Type, fileId.Value))
                    continue;

                switch (media.Type)
                {
                    case CommentMedia.Types.Audio:
                        await audioFileService.Remove(new(sessionId, new AudioFile { Id = fileId }));
                        break;
                    case CommentMedia.Types.Image:
                        await RemoveImageFile(sessionId, fileId);
                        break;
                    case CommentMedia.Types.Pdf:
                        await pdfFileService.Remove(new(sessionId, new PdfFile { Id = fileId }));
                        break;
                    case CommentMedia.Types.Video:
                        await videoFileService.Remove(new(sessionId, new VideoFile { Id = fileId }));
                        break;
                }

                continue;
            }

            // A blob without a persisted file row cannot be proven unique here. Forum uploads are
            // reclaimed through the upload-stage ledger; other raw blobs are retained conservatively.
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
}