using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;

namespace Crudspa.Content.Design.Server.Controllers;

[Route("api/content/design/forum-cover")]
public class ForumCoverController(
    ILogger<ForumCoverController> logger,
    IControllerWrappers controllerWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IImageFileService imageFileService,
    IBlobService blobService)
    : ControllerBase
{
    private const Int64 MaximumRequestBytes = ForumMediaPolicy.ImageMaxBytes + (1024L * 1024L);
    private static readonly SemaphoreSlim UploadConcurrency = new(2, 2);

    private String Connection => configService.Fetch().Database;

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaximumRequestBytes)]
    public async Task<ActionResult> Upload(Guid? portalId)
    {
        return await controllerWrappers.RequirePermission(Request, PermissionIds.Forums, async session =>
        {
            Guid? blobId = null;
            ImageFile? savedImage = null;
            var uploadSlot = false;

            if (!portalId.HasValue
                || !await ForumPortalAuthorize.Execute(Connection, session.Id, portalId))
                return Forbid();

            try
            {
                if (!Request.HasFormContentType)
                    return BadRequest("A multipart form upload is required.");

                if (Request.ContentLength > MaximumRequestBytes)
                    return StatusCode(StatusCodes.Status413PayloadTooLarge,
                        $"Forum cover upload is too large. The request limit is {MaximumRequestBytes:N0} bytes.");

                var requestSizeFeature = HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();

                if (requestSizeFeature is { IsReadOnly: false })
                    requestSizeFeature.MaxRequestBodySize = MaximumRequestBytes;
                else if (!Request.ContentLength.HasValue)
                    return StatusCode(StatusCodes.Status411LengthRequired,
                        "Content-Length is required for this upload.");

                await UploadConcurrency.WaitAsync(HttpContext.RequestAborted);
                uploadSlot = true;

                using var uploadTimeout = CancellationTokenSource.CreateLinkedTokenSource(HttpContext.RequestAborted);
                uploadTimeout.CancelAfter(TimeSpan.FromMinutes(5));

                var form = await Request.ReadFormAsync(new FormOptions
                {
                    MultipartBodyLengthLimit = ForumMediaPolicy.ImageMaxBytes,
                    MultipartBoundaryLengthLimit = 128,
                    MultipartHeadersCountLimit = 16,
                    MultipartHeadersLengthLimit = 16 * 1024,
                    ValueCountLimit = 1,
                    ValueLengthLimit = 1024,
                }, uploadTimeout.Token);
                var file = form.Files.GetFile("file");

                if (form.Count != 0 || file is null || form.Files.Count != 1)
                    return BadRequest("Exactly one file part named 'file' is required.");

                if (file.Length <= 0)
                    return BadRequest("Empty file.");

                if (file.Length > ForumMediaPolicy.ImageMaxBytes)
                    return BadRequest($"Forum cover is too large. The maximum is {ForumMediaPolicy.ImageMaxBytes:N0} bytes.");

                var fileName = Path.GetFileName(file.FileName);

                if (fileName.HasNothing() || fileName.Length > 150)
                    return BadRequest("File name is required and cannot be longer than 150 characters.");

                var extension = fileName.GetExtension();

                if (extension.HasNothing()
                    || !ForumMediaPolicy.ImageExtensions.HasAny(x => x.IsBasically(extension)))
                    return BadRequest("Forum covers must be supported raster images.");

                if (!ForumMediaPolicy.ImageContentTypes.HasAny(x => x.IsBasically(file.ContentType)))
                    return BadRequest("Invalid image content type.");

                await using var input = file.OpenReadStream();

                if (!await HasValidRasterSignature(input, extension, uploadTimeout.Token))
                    return BadRequest("File content does not match its image extension.");

                blobId = Guid.NewGuid();
                await blobService.AddStream(blobId.Value, input);

                if (!await blobService.Exists(blobId.Value))
                    throw new IOException("The uploaded forum cover could not be persisted.");

                var saveResponse = await fileService.SaveImage(new(session.Id, new ImageFile
                {
                    BlobId = blobId,
                    Name = fileName,
                    Format = extension,
                    OptimizedStatus = ImageFile.OptimizationStatus.None,
                }));

                savedImage = saveResponse.Value;
                if (!saveResponse.Ok || savedImage?.Id is null
                    || !ForumMediaPolicy.HasValidImageDimensions(savedImage.Width, savedImage.Height))
                {
                    if (savedImage?.Id is not null)
                        await RemoveSavedImage(session.Id, savedImage);
                    else if (blobId.HasValue)
                        await blobService.Remove(new Blob { Id = blobId });

                    blobId = null;
                    savedImage = null;
                    return BadRequest("Forum cover is not a valid supported raster image.");
                }

                blobId = null;
                return new JsonResult(savedImage);
            }
            catch (BadHttpRequestException ex)
            {
                logger.LogWarning(ex, "Rejected forum cover request. PortalId={PortalId}", portalId);
                return StatusCode(ex.StatusCode, "The upload request is malformed or too large.");
            }
            catch (InvalidDataException ex)
            {
                logger.LogWarning(ex, "Rejected malformed forum cover request. PortalId={PortalId}", portalId);
                return BadRequest("The multipart upload is malformed or too large.");
            }
            catch (OperationCanceledException) when (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                return StatusCode(StatusCodes.Status408RequestTimeout, "The upload timed out.");
            }
            catch (Exception ex)
            {
                if (savedImage?.Id is not null)
                    await RemoveSavedImage(session.Id, savedImage);
                else if (blobId.HasValue)
                    await blobService.Remove(new Blob { Id = blobId });

                logger.LogError(ex, "Exception while uploading forum cover. PortalId={PortalId}", portalId);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                if (uploadSlot)
                    UploadConcurrency.Release();
            }
        });
    }

    private async Task RemoveSavedImage(Guid? sessionId, ImageFile image)
    {
        await imageFileService.Remove(new(sessionId, image));

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

        foreach (var derivedBlobId in derivedBlobIds)
            await blobService.Remove(new Blob { Id = derivedBlobId });
    }

    private static async Task<Boolean> HasValidRasterSignature(Stream stream, String extension,
        CancellationToken cancellationToken)
    {
        if (!stream.CanSeek)
            return false;

        try
        {
            stream.Seek(0, SeekOrigin.Begin);
            var value = new Byte[16];
            var length = 0;

            while (length < value.Length)
            {
                var read = await stream.ReadAsync(value.AsMemory(length), cancellationToken);
                if (read == 0) break;
                length += read;
            }

            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => Matches(value, length, 0, 0xff, 0xd8, 0xff),
                ".png" => Matches(value, length, 0, 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a),
                ".gif" => MatchesAscii(value, length, 0, "GIF87a")
                          || MatchesAscii(value, length, 0, "GIF89a"),
                ".bmp" => MatchesAscii(value, length, 0, "BM"),
                ".tif" or ".tiff" => Matches(value, length, 0, 0x49, 0x49, 0x2a, 0x00)
                                     || Matches(value, length, 0, 0x4d, 0x4d, 0x00, 0x2a)
                                     || Matches(value, length, 0, 0x49, 0x49, 0x2b, 0x00)
                                     || Matches(value, length, 0, 0x4d, 0x4d, 0x00, 0x2b),
                ".webp" => MatchesAscii(value, length, 0, "RIFF")
                           && MatchesAscii(value, length, 8, "WEBP"),
                _ => false,
            };
        }
        finally
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
    }

    private static Boolean MatchesAscii(Byte[] value, Int32 length, Int32 offset, String expected)
    {
        if (length < offset + expected.Length)
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
        if (length < offset + expected.Length)
            return false;

        for (var index = 0; index < expected.Length; index++)
        {
            if (value[offset + index] != expected[index])
                return false;
        }

        return true;
    }
}