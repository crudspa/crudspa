using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;

namespace Crudspa.Content.Display.Server.Controllers;

[Route("api/content/display/forum-media")]
public class ForumMediaController(
    ILogger<ForumMediaController> logger,
    IControllerWrappers controllerWrappers,
    IForumRunService forumRunService,
    IForumMediaService forumMediaService,
    IBlobService blobService)
    : ControllerBase
{
    private const Int64 MaximumRequestBytes = ForumMediaPolicy.VideoMaxBytes + (1024L * 1024L);
    private static readonly SemaphoreSlim UploadConcurrency = new(2, 2);

    [HttpGet("forum-image")]
    public async Task<ActionResult> FetchForumImage(Guid? id)
    {
        return await controllerWrappers.RequireSession(Request, async session =>
        {
            try
            {
                var response = await forumRunService.FetchForum(new(session.Id, new Forum { Id = id }));
                var imageFile = response.Ok ? response.Value?.ImageFile : null;

                if (imageFile?.BlobId is null
                    || !ForumMediaPolicy.HasValidImageDimensions(imageFile.Width, imageFile.Height)
                    || !ForumMediaPolicy.ImageExtensions.HasAny(x => x.IsBasically(imageFile.Format)))
                    return NotFound();

                var stream = await blobService.TryFetchStream(imageFile.BlobId.Value);

                if (stream is null)
                    return NotFound();

                if (stream.CanSeek
                    && stream.Length is <= 0 or > ForumMediaPolicy.ImageMaxBytes)
                {
                    await stream.DisposeAsync();
                    return NotFound();
                }

                Response.Headers.CacheControl = "private, no-store";
                Response.Headers.XContentTypeOptions = "nosniff";
                Response.Headers.ContentSecurityPolicy = "default-src 'none'; sandbox";
                return File(stream, imageFile.Format.ToMimeType(), enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while fetching forum image. ForumId={ForumId}", id);
                return NotFound();
            }
        });
    }

    [HttpGet("fetch")]
    public async Task<ActionResult> Fetch(Guid? id, Boolean download = false)
    {
        return await controllerWrappers.RequireSession(Request, async session =>
        {
            try
            {
                var media = await forumMediaService.Fetch(session.Id, id);

                if (media?.BlobId is null)
                    return NotFound();

                var stream = await blobService.TryFetchStream(media.BlobId.Value);

                if (stream is null)
                    return NotFound();

                Response.Headers.CacheControl = "private, no-store";
                Response.Headers.XContentTypeOptions = "nosniff";
                Response.Headers.ContentSecurityPolicy = "default-src 'none'; sandbox";
                var mime = media.Format.ToMimeType();

                return download
                    ? File(stream, mime, media.Name, enableRangeProcessing: true)
                    : File(stream, mime, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while fetching forum media. CommentMediaId={CommentMediaId}", id);
                return NotFound();
            }
        });
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaximumRequestBytes)]
    public async Task<ActionResult> Upload(Guid? forumId, CommentMedia.Types? type)
    {
        return await controllerWrappers.RequireSession(Request, async session =>
        {
            Guid? blobId = null;
            var uploadSlot = false;

            if (session.User?.Id is null)
                return Forbid();

            try
            {
                if (!forumId.HasValue || !type.HasValue || !Enum.IsDefined(type.Value))
                    return BadRequest("Forum and media type are required.");

                if (!Request.HasFormContentType)
                    return BadRequest("A multipart form upload is required.");

                var maximumFileBytes = ForumMediaPolicy.MaxBytes(type.Value);
                var maximumRequestBytes = maximumFileBytes + (1024L * 1024L);

                if (Request.ContentLength > maximumRequestBytes)
                    return StatusCode(StatusCodes.Status413PayloadTooLarge,
                        $"Upload is too large. The request limit for {type.Value.ToString().ToLowerInvariant()} is {maximumRequestBytes:N0} bytes.");

                var requestSizeFeature = HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();

                if (requestSizeFeature is { IsReadOnly: false })
                    requestSizeFeature.MaxRequestBodySize = maximumRequestBytes;
                else if (!Request.ContentLength.HasValue)
                    return StatusCode(StatusCodes.Status411LengthRequired,
                        "Content-Length is required for this upload.");

                var forumResponse = await forumRunService.FetchForum(new(session.Id, new() { Id = forumId }));

                if (!forumResponse.Ok || forumResponse.Value is null)
                    return NotFound();

                await UploadConcurrency.WaitAsync(HttpContext.RequestAborted);
                uploadSlot = true;

                using var uploadTimeout = CancellationTokenSource.CreateLinkedTokenSource(HttpContext.RequestAborted);
                uploadTimeout.CancelAfter(TimeSpan.FromMinutes(15));

                var form = await Request.ReadFormAsync(new FormOptions
                {
                    MultipartBodyLengthLimit = maximumFileBytes,
                    MultipartBoundaryLengthLimit = 128,
                    MultipartHeadersCountLimit = 16,
                    MultipartHeadersLengthLimit = 16 * 1024,
                    ValueCountLimit = 1,
                    ValueLengthLimit = 1024,
                }, uploadTimeout.Token);
                var file = form.Files.GetFile("file");

                if (form.Count != 0 || form.Files.Count != 1 || file is null)
                    return BadRequest("Exactly one file part named 'file' is required.");

                if (file.Length <= 0)
                    return BadRequest("Empty file.");

                if (file.Length > maximumFileBytes)
                    return BadRequest($"File is too large. The maximum for {type.Value.ToString().ToLowerInvariant()} is {maximumFileBytes:N0} bytes.");

                var fileName = Path.GetFileName(file.FileName);

                if (fileName.HasNothing() || fileName.Length > 150)
                    return BadRequest("File name is required and cannot be longer than 150 characters.");

                var extension = fileName.GetExtension();

                if (extension.HasNothing() || !ForumMediaPolicy.Extensions(type.Value).HasAny(x => x.IsBasically(extension)))
                    return BadRequest("Invalid file type.");

                if (!ForumMediaPolicy.ContentTypes(type.Value).HasAny(x => x.IsBasically(file.ContentType)))
                    return BadRequest("Invalid content type.");

                await using var input = file.OpenReadStream();

                if (!await HasValidSignature(input, extension, uploadTimeout.Token))
                    return BadRequest("File content does not match its extension or media type.");

                await CleanupExpiredUploads();

                blobId = Guid.NewGuid();
                var upload = new ForumUploadStage
                {
                    BlobId = blobId,
                    Type = type.Value,
                    Name = fileName,
                    Format = extension,
                    ContentType = file.ContentType,
                    Bytes = file.Length,
                };

                switch (await forumMediaService.Stage(session.Id, forumId, upload))
                {
                    case ForumUploadStageResults.PendingQuotaExceeded:
                        return StatusCode(StatusCodes.Status429TooManyRequests,
                            $"Pending forum uploads are limited to {ForumMediaPolicy.MaxPendingUploadsPerUserPerForum} files and {ForumMediaPolicy.MaxPendingUploadBytesPerUserPerForum:N0} bytes per forum.");
                    case ForumUploadStageResults.DailyQuotaExceeded:
                        Response.Headers.RetryAfter = "3600";
                        return StatusCode(StatusCodes.Status429TooManyRequests,
                            $"Forum uploads are limited to {ForumMediaPolicy.MaxDailyUploadsPerUserPerForum} files and {ForumMediaPolicy.MaxDailyUploadBytesPerUserPerForum:N0} bytes per forum in a rolling 24-hour period.");
                    case ForumUploadStageResults.Staged:
                        break;
                    default:
                        return Forbid();
                }

                await blobService.AddStream(blobId.Value, input);

                if (!await blobService.Exists(blobId.Value))
                    throw new IOException("The uploaded forum media could not be persisted.");

                return new JsonResult(type.Value switch
                {
                    CommentMedia.Types.Audio => new AudioFile
                    {
                        BlobId = blobId,
                        Name = upload.Name,
                        Format = upload.Format,
                        OptimizedStatus = AudioFile.OptimizationStatus.None,
                    },
                    CommentMedia.Types.Image => new ImageFile
                    {
                        BlobId = blobId,
                        Name = upload.Name,
                        Format = upload.Format,
                        OptimizedStatus = ImageFile.OptimizationStatus.None,
                    },
                    CommentMedia.Types.Pdf => new PdfFile
                    {
                        BlobId = blobId,
                        Name = upload.Name,
                        Format = upload.Format,
                    },
                    CommentMedia.Types.Video => new VideoFile
                    {
                        BlobId = blobId,
                        Name = upload.Name,
                        Format = upload.Format,
                        OptimizedStatus = VideoFile.OptimizationStatus.None,
                    },
                    _ => throw new InvalidOperationException("Unsupported forum media type."),
                });
            }
            catch (BadHttpRequestException ex)
            {
                logger.LogWarning(ex, "Rejected forum media request. ForumId={ForumId}, Type={Type}", forumId, type);
                return StatusCode(ex.StatusCode, "The upload request is malformed or too large.");
            }
            catch (InvalidDataException ex)
            {
                logger.LogWarning(ex, "Rejected malformed forum media request. ForumId={ForumId}, Type={Type}", forumId, type);
                return BadRequest("The multipart upload is malformed or too large.");
            }
            catch (OperationCanceledException) when (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                return StatusCode(StatusCodes.Status408RequestTimeout, "The upload timed out.");
            }
            catch (Exception ex)
            {
                if (blobId.HasValue)
                {
                    try
                    {
                        await forumMediaService.Discard(session.Id, blobId);
                        await blobService.Remove(new Blob { Id = blobId });
                    }
                    catch (Exception cleanupException)
                    {
                        logger.LogWarning(cleanupException, "Could not clean up a failed forum upload. BlobId={BlobId}", blobId);
                    }
                }

                logger.LogError(ex, "Exception while uploading forum media. ForumId={ForumId}, Type={Type}", forumId, type);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                if (uploadSlot)
                    UploadConcurrency.Release();
            }
        });
    }

    private async Task CleanupExpiredUploads()
    {
        foreach (var expiredBlobId in await forumMediaService.FetchExpired())
        {
            await blobService.Remove(new Blob { Id = expiredBlobId });
            if (!await blobService.Exists(expiredBlobId))
                await forumMediaService.DiscardExpired(expiredBlobId);
        }
    }

    public static async Task<Boolean> HasValidSignature(Stream stream, String extension, CancellationToken cancellationToken)
    {
        const Int32 signatureBufferBytes = 512;

        if (!stream.CanSeek)
            return false;

        try
        {
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new Byte[signatureBufferBytes];
            var length = 0;

            while (length < buffer.Length)
            {
                var read = await stream.ReadAsync(buffer.AsMemory(length), cancellationToken);

                if (read == 0)
                    break;

                length += read;
            }

            return HasValidSignature(buffer, length, extension);
        }
        finally
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
    }

    private static Boolean HasValidSignature(Byte[] value, Int32 length, String extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => Matches(value, length, 0, 0xff, 0xd8, 0xff),
            ".png" => Matches(value, length, 0, 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a),
            ".gif" => MatchesAscii(value, length, 0, "GIF87a") || MatchesAscii(value, length, 0, "GIF89a"),
            ".bmp" => MatchesAscii(value, length, 0, "BM"),
            ".tif" or ".tiff" => IsTiff(value, length),
            ".webp" => IsRiff(value, length, "WEBP"),
            ".pdf" => MatchesAscii(value, length, 0, "%PDF-"),
            ".mp3" => MatchesAscii(value, length, 0, "ID3") || IsMpegAudioFrame(value, length),
            ".m4a" or ".mp4" or ".m4v" => IsIsoBaseMedia(value, length),
            ".mov" => IsIsoBaseMedia(value, length) || IsQuickTime(value, length),
            ".aac" => IsAac(value, length),
            ".wav" => IsRiff(value, length, "WAVE") || IsRifx(value, length, "WAVE"),
            ".flac" => MatchesAscii(value, length, 0, "fLaC"),
            ".ogg" or ".oga" => MatchesAscii(value, length, 0, "OggS"),
            ".opus" => MatchesAscii(value, length, 0, "OggS") && ContainsAscii(value, length, "OpusHead"),
            ".aif" or ".aiff" => IsAiff(value, length),
            ".wma" or ".wmv" => IsAsf(value, length),
            ".avi" => IsRiff(value, length, "AVI "),
            ".flv" => IsFlv(value, length),
            ".webm" => IsEbml(value, length) && ContainsAscii(value, length, "webm"),
            ".mkv" => IsEbml(value, length) && ContainsAscii(value, length, "matroska"),
            ".mpeg" or ".mpg" => IsMpegVideo(value, length),
            ".ts" => HasTransportStreamSync(value, length, 0, 188) || HasTransportStreamSync(value, length, 0, 204),
            ".m2ts" => HasTransportStreamSync(value, length, 4, 192),
            _ => false,
        };
    }

    private static Boolean IsTiff(Byte[] value, Int32 length)
    {
        return Matches(value, length, 0, 0x49, 0x49, 0x2a, 0x00)
               || Matches(value, length, 0, 0x4d, 0x4d, 0x00, 0x2a)
               || Matches(value, length, 0, 0x49, 0x49, 0x2b, 0x00)
               || Matches(value, length, 0, 0x4d, 0x4d, 0x00, 0x2b);
    }

    private static Boolean IsMpegAudioFrame(Byte[] value, Int32 length)
    {
        return length >= 2 && value[0] == 0xff && (value[1] & 0xe0) == 0xe0;
    }

    private static Boolean IsAac(Byte[] value, Int32 length)
    {
        return MatchesAscii(value, length, 0, "ADIF")
               || length >= 2 && value[0] == 0xff && (value[1] & 0xf6) == 0xf0;
    }

    private static Boolean IsIsoBaseMedia(Byte[] value, Int32 length)
    {
        if (!MatchesAscii(value, length, 4, "ftyp"))
            return false;

        var boxLength = ReadBigEndianUInt32(value);
        return boxLength == 1 ? length >= 16 : boxLength >= 8;
    }

    private static Boolean IsQuickTime(Byte[] value, Int32 length)
    {
        if (length < 8 || ReadBigEndianUInt32(value) < 8)
            return false;

        return MatchesAscii(value, length, 4, "moov")
               || MatchesAscii(value, length, 4, "mdat")
               || MatchesAscii(value, length, 4, "free")
               || MatchesAscii(value, length, 4, "skip")
               || MatchesAscii(value, length, 4, "wide")
               || MatchesAscii(value, length, 4, "pnot");
    }

    private static UInt32 ReadBigEndianUInt32(Byte[] value)
    {
        return ((UInt32)value[0] << 24)
               | ((UInt32)value[1] << 16)
               | ((UInt32)value[2] << 8)
               | value[3];
    }

    private static Boolean IsAiff(Byte[] value, Int32 length)
    {
        return MatchesAscii(value, length, 0, "FORM")
               && (MatchesAscii(value, length, 8, "AIFF") || MatchesAscii(value, length, 8, "AIFC"));
    }

    private static Boolean IsAsf(Byte[] value, Int32 length)
    {
        return Matches(value, length, 0,
            0x30, 0x26, 0xb2, 0x75, 0x8e, 0x66, 0xcf, 0x11,
            0xa6, 0xd9, 0x00, 0xaa, 0x00, 0x62, 0xce, 0x6c);
    }

    private static Boolean IsFlv(Byte[] value, Int32 length)
    {
        return MatchesAscii(value, length, 0, "FLV")
               && length >= 9
               && value[3] == 1
               && (value[4] & 0xfa) == 0;
    }

    private static Boolean IsEbml(Byte[] value, Int32 length)
    {
        return Matches(value, length, 0, 0x1a, 0x45, 0xdf, 0xa3);
    }

    private static Boolean IsMpegVideo(Byte[] value, Int32 length)
    {
        for (var index = 0; index <= length - 4; index++)
        {
            if (Matches(value, length, index, 0x00, 0x00, 0x01, 0xba)
                || Matches(value, length, index, 0x00, 0x00, 0x01, 0xb3))
                return true;
        }

        return false;
    }

    private static Boolean HasTransportStreamSync(Byte[] value, Int32 length, Int32 offset, Int32 packetBytes)
    {
        return length > offset + (packetBytes * 2)
               && value[offset] == 0x47
               && value[offset + packetBytes] == 0x47
               && value[offset + (packetBytes * 2)] == 0x47;
    }

    private static Boolean IsRiff(Byte[] value, Int32 length, String format)
    {
        return MatchesAscii(value, length, 0, "RIFF") && MatchesAscii(value, length, 8, format);
    }

    private static Boolean IsRifx(Byte[] value, Int32 length, String format)
    {
        return MatchesAscii(value, length, 0, "RIFX") && MatchesAscii(value, length, 8, format);
    }

    private static Boolean ContainsAscii(Byte[] value, Int32 length, String expected)
    {
        for (var offset = 0; offset <= length - expected.Length; offset++)
        {
            if (MatchesAscii(value, length, offset, expected))
                return true;
        }

        return false;
    }

    private static Boolean MatchesAscii(Byte[] value, Int32 length, Int32 offset, String expected)
    {
        if (offset < 0 || length < offset + expected.Length)
            return false;

        for (var index = 0; index < expected.Length; index++)
        {
            if (value[offset + index] != (Byte)expected[index])
                return false;
        }

        return true;
    }

    private static Boolean Matches(Byte[] value, Int32 length, Int32 offset, params Byte[] expected)
    {
        if (offset < 0 || length < offset + expected.Length)
            return false;

        for (var index = 0; index < expected.Length; index++)
        {
            if (value[offset + index] != expected[index])
                return false;
        }

        return true;
    }
}