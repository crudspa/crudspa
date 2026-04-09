using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Framework.Core.Server.Controllers;

[Route("api/framework/core/audio-file")]
public class AudioFileController(
    ILogger<AudioFileController> logger,
    IControllerWrappers controllerWrappers,
    IBlobService blobService,
    IAudioFileService audioFileService)
    : ControllerBase
{
    [HttpGet("fetch")]
    public async Task<ActionResult> Fetch(Guid? id, Guid? version, Boolean download = false, Boolean original = false)
    {
        return await controllerWrappers.RequireSession(Request, async session =>
        {
            try
            {
                var response = await audioFileService.Fetch(new(session.Id, new() { Id = id }));

                if (!response.Ok)
                {
                    logger.LogWarning("Audio file not found. Id: {id}", id);
                    return NotFound();
                }

                var audioFile = response.Value;

                var candidates = original
                    ? new() { (audioFile.BlobId, audioFile.Format, false)! }
                    : new List<(Guid? BlobId, String Format, Boolean IsOptimized)>
                    {
                        (audioFile.OptimizedBlobId, audioFile.OptimizedFormat, true)!,
                        (audioFile.BlobId, audioFile.Format, false)!,
                    };

                foreach (var candidate in candidates)
                {
                    if (!candidate.BlobId.HasValue) continue;

                    var stream = await blobService.TryFetchStream(candidate.BlobId.Value);
                    if (stream is null) continue;

                    Response.SetCacheHeaders(candidate.IsOptimized || version.HasValue);

                    var mime = candidate.Format.ToMimeType();

                    return download
                        ? File(stream, mime, audioFile.Name, enableRangeProcessing: true)
                        : File(stream, mime, enableRangeProcessing: true);
                }

                logger.LogWarning("Audio blob(s) missing. Id: {id}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while fetching audio file. Id: {id}", id);
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

                if (!Constants.AllowedAudioExtensions.HasAny(x => x.IsBasically(extension)))
                    return BadRequest("Invalid file type.");

                if (!Constants.AllowedAudioContentTypes.HasAny(x => x.IsBasically(file.ContentType)))
                    return BadRequest("Invalid content type.");

                var blobId = Guid.NewGuid();
                await using var input = file.OpenReadStream();
                await blobService.AddStream(blobId, input);

                var audioFile = new AudioFile
                {
                    BlobId = blobId,
                    Name = file.FileName,
                    Format = extension,
                    OptimizedStatus = AudioFile.OptimizationStatus.None,
                };

                return new JsonResult(audioFile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while uploading audio file.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        });
    }
}