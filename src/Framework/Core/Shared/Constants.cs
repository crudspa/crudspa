namespace Crudspa.Framework.Core.Shared;

public static class Constants
{
    public static class ErrorMessages
    {
        public const String GenericError = "An unexpected error occurred.";
        public const String PermissionDenied = "Permission denied.";
        public const String ServiceCallFailed = "Error calling server.";
        public const String ValidationException = "Error validating data.";
    }

    public static class AccessDeniedEventTypes
    {
        public const String PermissionDenied = "Permission Denied";
        public const String AuthFailed = "Authentication Failed";
        public const String AccessCodeFailed = "Access Code Failed";
        public const String Unknown = "Unknown";
    }

    public static class EmailLayoutKeys
    {
        public const String AccessCode = "AccessCode";
        public const String Message = "Message";
    }

    public static class CookieKeys
    {
        public const String SessionId = "SessionId";
        public const String Username = "Username";
    }

    public const String DefaultTimeZone = "America/New_York";

    public static readonly String[] AllowedAudioContentTypes = ["audio/mpeg", "audio/mp4", "audio/aac", "audio/x-aac", "audio/wav", "audio/x-wav", "audio/flac", "audio/ogg", "application/ogg", "audio/opus", "audio/x-aiff", "audio/x-ms-wma"];
    public static readonly String[] AllowedAudioExtensions = [".mp3", ".m4a", ".aac", ".wav", ".flac", ".ogg", ".oga", ".opus", ".aif", ".aiff", ".wma"];
    public static readonly String[] AllowedFontContentTypes = ["font/ttf", "font/otf", "application/font-woff", "application/font-woff2", "font/woff", "font/woff2", "application/octet-stream"];
    public static readonly String[] AllowedFontExtensions = [".otf", ".ttf", ".woff", ".woff2"];
    public static readonly String[] AllowedImageContentTypes = ["image/jpeg", "image/png", "image/gif", "image/bmp", "image/svg+xml", "image/tiff", "image/webp"];
    public static readonly String[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".tif", ".tiff", ".webp"];
    public static readonly String[] AllowedPdfContentTypes = ["application/pdf"];
    public static readonly String[] AllowedPdfExtensions = [".pdf"];
    public static readonly String[] AllowedVideoContentTypes = ["video/mp4", "video/quicktime", "video/x-m4v", "video/x-msvideo", "video/x-ms-wmv", "video/x-flv", "video/webm", "video/x-matroska", "video/mpeg", "video/mp2t", "application/octet-stream"];
    public static readonly String[] AllowedVideoExtensions = [".mp4", ".mov", ".m4v", ".avi", ".wmv", ".flv", ".webm", ".mkv", ".mpeg", ".mpg", ".ts", ".m2ts"];

    public static String Join(params IEnumerable<String>[] groups) => String.Join(",", groups.SelectMany(x => x));
}