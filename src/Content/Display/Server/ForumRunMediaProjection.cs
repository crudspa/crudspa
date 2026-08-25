namespace Crudspa.Content.Display.Server;

public static class ForumRunMediaProjection
{
    public static Boolean TryHydrateUnchanged(CommentMedia media, CommentMedia? existing)
    {
        if (HasIncomingFileReference(media))
            return true;

        if (existing is null || media.Id != existing.Id || media.Type != existing.Type)
            return false;

        switch (media.Type)
        {
            case CommentMedia.Types.Audio:
                media.AudioFile = existing.AudioFile;
                break;
            case CommentMedia.Types.Image:
                media.ImageFile = existing.ImageFile;
                break;
            case CommentMedia.Types.Pdf:
                media.PdfFile = existing.PdfFile;
                break;
            case CommentMedia.Types.Video:
                media.VideoFile = existing.VideoFile;
                break;
            default:
                return false;
        }

        return true;
    }

    private static Boolean HasIncomingFileReference(CommentMedia media)
    {
        return media.Type switch
        {
            CommentMedia.Types.Audio => media.AudioFile.Id.HasValue || media.AudioFile.BlobId.HasValue,
            CommentMedia.Types.Image => media.ImageFile.Id.HasValue || media.ImageFile.BlobId.HasValue,
            CommentMedia.Types.Pdf => media.PdfFile.Id.HasValue || media.PdfFile.BlobId.HasValue,
            CommentMedia.Types.Video => media.VideoFile.Id.HasValue || media.VideoFile.BlobId.HasValue,
            _ => false,
        };
    }
}