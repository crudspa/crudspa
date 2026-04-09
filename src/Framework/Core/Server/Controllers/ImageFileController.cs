using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Framework.Core.Server.Controllers;

[Route("api/framework/core/image-file")]
public class ImageFileController(
    ILogger<ImageFileController> logger,
    IControllerWrappers controllerWrappers,
    IBlobService blobService,
    IImageFileService imageFileService)
    : ControllerBase
{
    [HttpGet("fetch")]
    public async Task<ActionResult> Fetch(Guid? id, Int32? width, Guid? version, Boolean download = false, Boolean original = false)
    {
        return await controllerWrappers.RequireSession(Request, async session =>
        {
            try
            {
                var response = await imageFileService.Fetch(new(session.Id, new() { Id = id }));

                if (!response.Ok)
                {
                    logger.LogWarning("Image file not found. Id: {id}", id);
                    return NotFound();
                }

                var imageFile = response.Value;

                foreach (var candidate in BuildBlobCandidates(imageFile, width, original))
                {
                    if (!candidate.BlobId.HasValue) continue;

                    var stream = await blobService.TryFetchStream(candidate.BlobId.Value);
                    if (stream is null) continue;

                    Response.SetCacheHeaders(candidate.IsOptimized || version.HasValue);

                    var mime = candidate.Format.ToMimeType();

                    return download
                        ? File(stream, mime, imageFile.Name, enableRangeProcessing: true)
                        : File(stream, mime, enableRangeProcessing: true);
                }

                logger.LogWarning("No reachable blob for image file. Id: {id}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while fetching image file. Id: {id}", id);
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

                if (!Constants.AllowedImageExtensions.HasAny(x => x.IsBasically(extension)))
                    return BadRequest("Invalid file type.");

                if (!Constants.AllowedImageContentTypes.HasAny(x => x.IsBasically(file.ContentType)))
                    return BadRequest("Invalid content type.");

                var blobId = Guid.NewGuid();
                await using var input = file.OpenReadStream();
                await blobService.AddStream(blobId, input);

                var imageFile = new ImageFile
                {
                    BlobId = blobId,
                    Name = file.FileName,
                    Format = extension,
                    OptimizedStatus = ImageFile.OptimizationStatus.None,
                };

                return new JsonResult(imageFile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while uploading image file.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        });
    }

    private static IEnumerable<(Guid? BlobId, String Format, Boolean IsOptimized)> BuildBlobCandidates(ImageFile imageFile, Int32? requestedWidth, Boolean original)
    {
        if (original)
        {
            yield return (imageFile.BlobId, imageFile.Format ?? String.Empty, false);
            yield break;
        }

        if (requestedWidth.HasValue)
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
                var match = available.FirstOrDefault(x => x.width >= requestedWidth.Value);
                var chosen = match.blobId ?? available.Last().blobId;

                if (chosen.HasValue)
                {
                    var chosenFormat = imageFile.OptimizedFormat.HasSomething()
                        ? imageFile.OptimizedFormat!
                        : imageFile.Format ?? String.Empty;

                    yield return (chosen, chosenFormat, true);
                }
            }
        }

        if (imageFile.OptimizedStatus == ImageFile.OptimizationStatus.Succeeded && imageFile.OptimizedBlobId.HasValue && imageFile.OptimizedFormat.HasSomething())
            yield return (imageFile.OptimizedBlobId, imageFile.OptimizedFormat!, true);

        yield return (imageFile.BlobId, imageFile.Format ?? String.Empty, false);
    }
}