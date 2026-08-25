namespace Crudspa.Content.Display.Shared.Contracts.Data;

public static class ForumMediaPolicy
{
    public const Int32 MaxAttachmentsPerComment = 10;
    public const Int32 MaxPendingUploadsPerUserPerForum = 20;
    public const Int32 MaxDailyUploadsPerUserPerForum = 40;
    public const Int64 MaxFileBytes = 4L * 1024L * 1024L * 1024L;
    public const Int64 MaxPendingUploadBytesPerUserPerForum = MaxFileBytes * MaxPendingUploadsPerUserPerForum;
    public const Int64 MaxDailyUploadBytesPerUserPerForum = MaxFileBytes * MaxDailyUploadsPerUserPerForum;

    public const Int64 ImageMaxBytes = MaxFileBytes;
    public const Int32 ImageMaxDimension = 12000;
    public const Int64 ImageMaxPixels = 40_000_000;
    public const Int64 PdfMaxBytes = MaxFileBytes;
    public const Int64 AudioMaxBytes = MaxFileBytes;
    public const Int64 VideoMaxBytes = MaxFileBytes;

    public static readonly String[] AudioContentTypes = [.. Constants.AllowedAudioContentTypes];
    public static readonly String[] AudioExtensions = [.. Constants.AllowedAudioExtensions];
    public static readonly String[] ImageContentTypes = ["image/jpeg", "image/png", "image/gif", "image/bmp", "image/tiff", "image/webp"];
    public static readonly String[] ImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".webp"];
    public static readonly String[] PdfContentTypes = [.. Constants.AllowedPdfContentTypes];
    public static readonly String[] PdfExtensions = [.. Constants.AllowedPdfExtensions];
    public static readonly String[] VideoContentTypes = [.. Constants.AllowedVideoContentTypes];
    public static readonly String[] VideoExtensions = [.. Constants.AllowedVideoExtensions];

    public static Int64 MaxBytes(CommentMedia.Types type) => type switch
    {
        CommentMedia.Types.Audio => AudioMaxBytes,
        CommentMedia.Types.Image => ImageMaxBytes,
        CommentMedia.Types.Pdf => PdfMaxBytes,
        CommentMedia.Types.Video => VideoMaxBytes,
        _ => 0,
    };

    public static IReadOnlyList<String> Extensions(CommentMedia.Types type) => type switch
    {
        CommentMedia.Types.Audio => AudioExtensions,
        CommentMedia.Types.Image => ImageExtensions,
        CommentMedia.Types.Pdf => PdfExtensions,
        CommentMedia.Types.Video => VideoExtensions,
        _ => [],
    };

    public static IReadOnlyList<String> ContentTypes(CommentMedia.Types type) => type switch
    {
        CommentMedia.Types.Audio => AudioContentTypes,
        CommentMedia.Types.Image => ImageContentTypes,
        CommentMedia.Types.Pdf => PdfContentTypes,
        CommentMedia.Types.Video => VideoContentTypes,
        _ => [],
    };

    public static Boolean HasValidImageDimensions(Int32? width, Int32? height) =>
        width is > 0 and <= ImageMaxDimension
        && height is > 0 and <= ImageMaxDimension
        && (Int64)width.Value * height.Value <= ImageMaxPixels;
}