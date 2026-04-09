using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Framework.Core.Server.Controllers;

[Route("api/framework/core/video-file")]
public class VideoFileController(
    ILogger<VideoFileController> logger,
    IControllerWrappers controllerWrappers,
    IBlobService blobService,
    IVideoFileService videoFileService)
    : ControllerBase
{
    [HttpGet("fetch")]
    public async Task<ActionResult> Fetch(Guid? id, Guid? version, Boolean download = false, Boolean original = false)
    {
        return await controllerWrappers.RequireSession(Request, async session =>
        {
            try
            {
                var response = await videoFileService.Fetch(new(session.Id, new() { Id = id }));

                if (!response.Ok)
                {
                    logger.LogWarning("Video file not found. Id: {id}", id);
                    return NotFound();
                }

                var videoFile = response.Value;

                var candidates = original
                    ? new() { (videoFile.BlobId, videoFile.Format, false)! }
                    : new List<(Guid? BlobId, String Format, Boolean IsOptimized)>
                    {
                        (videoFile.OptimizedBlobId, videoFile.OptimizedFormat, true)!,
                        (videoFile.BlobId, videoFile.Format, false)!,
                    };

                foreach (var candidate in candidates)
                {
                    if (!candidate.BlobId.HasValue) continue;

                    var stream = await blobService.TryFetchStream(candidate.BlobId.Value);
                    if (stream is null) continue;

                    Response.SetCacheHeaders(candidate.IsOptimized || version.HasValue);

                    var mime = candidate.Format.ToMimeType();

                    return download
                        ? File(stream, mime, videoFile.Name, enableRangeProcessing: true)
                        : File(stream, mime, enableRangeProcessing: true);
                }

                logger.LogWarning("Video blob(s) missing. Id: {id}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while fetching video file. Id: {id}", id);
                return NotFound();
            }
        });
    }

    [HttpGet("fetch-poster")]
    public async Task<ActionResult> FetchPoster(Guid? id, Guid? version)
    {
        return await controllerWrappers.RequireSession(Request, async session =>
        {
            try
            {
                var response = await videoFileService.Fetch(new(session.Id, new() { Id = id }));

                if (!response.Ok)
                {
                    Response.SetCacheHeaders(false);
                    return NotFound();
                }

                var videoFile = response.Value;

                if (!videoFile.PosterBlobId.HasValue || videoFile.PosterFormat.HasNothing())
                {
                    Response.SetCacheHeaders(false);
                    return NotFound();
                }

                var stream = await blobService.TryFetchStream(videoFile.PosterBlobId.Value);

                if (stream is null)
                {
                    Response.SetCacheHeaders(false);
                    return NotFound();
                }

                Response.SetCacheHeaders(version.HasValue);

                return File(stream, videoFile.PosterFormat.ToMimeType(), enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while fetching video poster. Id: {id}", id);
                return NotFound();
            }
        });
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Upload([FromForm(Name = "file")] IFormFile? file)
    {
        return await controllerWrappers.RequireSession(Request, async session =>
        {
            try
            {
                if (file is null)
                    return BadRequest("No file part named 'file' was uploaded.");

                if (file.Length <= 0)
                    return BadRequest("Empty file.");

                var extension = file.FileName.GetExtension();

                if (extension.HasNothing())
                    return BadRequest("File extension not found.");

                if (!Constants.AllowedVideoExtensions.HasAny(x => x.IsBasically(extension)))
                    return BadRequest("Invalid file type.");

                if (!Constants.AllowedVideoContentTypes.HasAny(x => x.IsBasically(file.ContentType)))
                    return BadRequest("Invalid content type.");

                var blobId = Guid.NewGuid();
                await using var input = file.OpenReadStream();
                await blobService.AddStream(blobId, input);

                var videoFile = new VideoFile
                {
                    BlobId = blobId,
                    Name = file.FileName,
                    Format = extension,
                    OptimizedStatus = VideoFile.OptimizationStatus.None,
                };

                return new JsonResult(videoFile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while uploading video file.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        });
    }
}