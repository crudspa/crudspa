namespace Crudspa.Framework.Core.Shared.Extensions;

public static class MediaFileEx
{
    extension(ImageFile? imageFile)
    {
        public Guid? CacheVersion(Int32? width = null)
        {
            if (imageFile is null)
                return null;

            if (width is > 0)
            {
                List<(Int32 width, Guid? blobId)> widthsToBlob =
                [
                    (96, imageFile.Resized96BlobId),
                    (192, imageFile.Resized192BlobId),
                    (360, imageFile.Resized360BlobId),
                    (540, imageFile.Resized540BlobId),
                    (720, imageFile.Resized720BlobId),
                    (1080, imageFile.Resized1080BlobId),
                    (1440, imageFile.Resized1440BlobId),
                    (1920, imageFile.Resized1920BlobId),
                    (3840, imageFile.Resized3840BlobId),
                ];

                var available = widthsToBlob.Where(x => x.blobId.HasValue).OrderBy(x => x.width).ToList();

                if (available.Count > 0)
                {
                    var match = available.FirstOrDefault(x => x.width >= width.Value);
                    var chosen = match.blobId ?? available.Last().blobId;

                    if (chosen.HasValue)
                        return chosen;
                }
            }

            if (imageFile.OptimizedBlobId.HasValue)
                return imageFile.OptimizedBlobId;

            if (imageFile.BlobId.HasValue)
                return imageFile.BlobId;

            return imageFile.Id;
        }

        public String FetchUrl(Int32? width = null, Boolean download = false)
        {
            if (imageFile?.Id is not { } imageFileId)
                return String.Empty;

            return BuildFetchUrl(
                "/api/framework/core/image-file/fetch",
                imageFileId,
                imageFile.CacheVersion(width),
                ("width", width is > 0 ? $"{width.Value:D}" : null),
                ("download", download ? "true" : null));
        }
    }

    extension(AudioFile? audioFile)
    {
        public Guid? CacheVersion()
        {
            if (audioFile is null)
                return null;

            if (audioFile.OptimizedBlobId.HasValue)
                return audioFile.OptimizedBlobId;

            if (audioFile.BlobId.HasValue)
                return audioFile.BlobId;

            return audioFile.Id;
        }

        public String FetchUrl(Boolean download = false)
        {
            if (audioFile?.Id is not { } audioFileId)
                return String.Empty;

            return BuildFetchUrl(
                "/api/framework/core/audio-file/fetch",
                audioFileId,
                audioFile.CacheVersion(),
                ("download", download ? "true" : null));
        }
    }

    extension(VideoFile? videoFile)
    {
        public Guid? CacheVersion()
        {
            if (videoFile is null)
                return null;

            if (videoFile.OptimizedBlobId.HasValue)
                return videoFile.OptimizedBlobId;

            if (videoFile.BlobId.HasValue)
                return videoFile.BlobId;

            return videoFile.Id;
        }

        public String FetchUrl(Boolean download = false)
        {
            if (videoFile?.Id is not { } videoFileId)
                return String.Empty;

            return BuildFetchUrl(
                "/api/framework/core/video-file/fetch",
                videoFileId,
                videoFile.CacheVersion(),
                ("download", download ? "true" : null));
        }

        public Guid? PosterCacheVersion()
        {
            if (videoFile is null)
                return null;

            if (videoFile.PosterBlobId.HasValue)
                return videoFile.PosterBlobId;

            return null;
        }

        public String FetchPosterUrl()
        {
            if (videoFile?.Id is not { } videoFileId)
                return String.Empty;

            return BuildFetchUrl(
                "/api/framework/core/video-file/fetch-poster",
                videoFileId,
                videoFile.PosterCacheVersion());
        }
    }

    extension(PdfFile? pdfFile)
    {
        public Guid? CacheVersion()
        {
            if (pdfFile is null)
                return null;

            if (pdfFile.BlobId.HasValue)
                return pdfFile.BlobId;

            return pdfFile.Id;
        }

        public String FetchUrl(Boolean download = false)
        {
            if (pdfFile?.Id is not { } pdfFileId)
                return String.Empty;

            return BuildFetchUrl(
                "/api/framework/core/pdf-file/fetch",
                pdfFileId,
                pdfFile.CacheVersion(),
                ("download", download ? "true" : null));
        }
    }

    extension(FontFile? fontFile)
    {
        public Guid? CacheVersion()
        {
            if (fontFile is null)
                return null;

            if (fontFile.BlobId.HasValue)
                return fontFile.BlobId;

            return fontFile.Id;
        }

        public String FetchUrl(Boolean download = false)
        {
            if (fontFile?.Id is not { } fontFileId)
                return String.Empty;

            return BuildFetchUrl(
                $"/api/framework/core/font-file/{fontFileId:D}",
                fontFile.CacheVersion(),
                ("download", download ? "true" : null));
        }
    }

    private static String BuildFetchUrl(
        String path,
        Guid fileId,
        Guid? version,
        params (String key, String? value)[] extraQueryParameters)
    {
        var queryParameters = new List<(String key, String? value)>
        {
            ("id", $"{fileId:D}"),
            ("version", version.HasValue ? $"{version.Value:D}" : null),
        };

        queryParameters.AddRange(extraQueryParameters);

        return BuildFetchUrl(path, queryParameters.ToArray());
    }

    private static String BuildFetchUrl(
        String path,
        Guid? version,
        params (String key, String? value)[] extraQueryParameters)
    {
        var queryParameters = extraQueryParameters.ToList();

        queryParameters.Add(("version", version.HasValue ? $"{version.Value:D}" : null));

        return BuildFetchUrl(path, queryParameters.ToArray());
    }

    private static String BuildFetchUrl(
        String path,
        params (String key, String? value)[] queryParameters)
    {
        var nonNullQueryParameters = queryParameters
            .Where(x => x.value.HasSomething())
            .Select(x => $"{x.key}={Uri.EscapeDataString(x.value!)}");

        var queryString = String.Join("&", nonNullQueryParameters);

        return queryString.HasNothing()
            ? path
            : $"{path}?{queryString}";
    }
}