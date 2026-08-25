using Crudspa.Content.Display.Server.Controllers;
using Crudspa.Content.Display.Server.Contracts.Behavior;
using Crudspa.Content.Display.Server.Contracts.Data;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Server.Controllers;

[Route("api/content/design/forum-comment-media")]
public class ForumCommentMediaController(
    ILogger<ForumCommentMediaController> logger,
    IControllerWrappers controllerWrappers,
    IServerConfigService configService,
    IForumMediaService forumMediaService,
    IBlobService blobService)
    : ControllerBase
{
    private const Int64 MaximumRequestBytes = ForumMediaPolicy.VideoMaxBytes + (1024L * 1024L);
    private static readonly SemaphoreSlim UploadConcurrency = new(2, 2);

    private String Connection => configService.Fetch().Database;

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaximumRequestBytes)]
    public async Task<ActionResult> Upload(Guid? threadId, CommentMedia.Types? type)
    {
        return await controllerWrappers.RequirePermission(Request, PermissionIds.Forums, async session =>
        {
            Guid? blobId = null;
            var uploadSlot = false;

            try
            {
                if (!threadId.HasValue || !type.HasValue || !Enum.IsDefined(type.Value))
                    return BadRequest("Thread and media type are required.");

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

                var thread = await ThreadSelect.Execute(Connection, session.Id, new Thread { Id = threadId });

                if (thread?.ForumId is null)
                    return Forbid();

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

                if (!await ForumMediaController.HasValidSignature(input, extension, uploadTimeout.Token))
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

                switch (await ForumRunUploadStage.Insert(Connection, session.Id, thread.ForumId, upload, []))
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
                logger.LogWarning(ex, "Rejected forum comment media request. ThreadId={ThreadId}, Type={Type}", threadId, type);
                return StatusCode(ex.StatusCode, "The upload request is malformed or too large.");
            }
            catch (InvalidDataException ex)
            {
                logger.LogWarning(ex, "Rejected malformed forum comment media request. ThreadId={ThreadId}, Type={Type}", threadId, type);
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
                        logger.LogWarning(cleanupException,
                            "Could not clean up a failed forum comment upload. BlobId={BlobId}", blobId);
                    }
                }

                logger.LogError(ex, "Exception while uploading forum comment media. ThreadId={ThreadId}, Type={Type}",
                    threadId, type);
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
}