using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Framework.Core.Server.Controllers;

[Route("api/framework/core/font-file")]
public class FontFileController(
    ILogger<FontFileController> logger,
    IControllerWrappers controllerWrappers,
    IBlobService blobService,
    IFontFileService fontFileService)
    : ControllerBase
{
    [HttpGet("fetch")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Fetch(Guid? id, Boolean download = false, Boolean original = false)
    {
        try
        {
            var response = await fontFileService.Fetch(new(new() { Id = id }));

            if (!response.Ok)
            {
                logger.LogWarning("Font file not found. Id: {id}", id);
                return NotFound();
            }

            var fontFile = response.Value;

            var blobId = fontFile.BlobId;
            if (!blobId.HasValue)
            {
                logger.LogWarning("Blob id missing for font file. Id: {id}", id);
                return NotFound();
            }

            var stream = await blobService.TryFetchStream(blobId.Value);
            if (stream is null)
            {
                logger.LogWarning("Blob for font file not found. Id: {id}", id);
                return NotFound();
            }

            Response.SetCacheHeaders(true);

            var mime = fontFile.Format.ToMimeType();

            return download
                ? File(stream, mime, fontFile.Name, enableRangeProcessing: true)
                : File(stream, mime, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while fetching font file. Id: {id}", id);
            return NotFound();
        }
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Upload([FromForm(Name = "file")] IFormFile? file)
    {
        return await controllerWrappers.RequireSession(Request, async _ =>
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

                if (!Constants.AllowedFontExtensions.HasAny(x => x.IsBasically(extension)))
                    return BadRequest("Invalid file type.");

                if (!Constants.AllowedFontContentTypes.HasAny(x => x.IsBasically(file.ContentType)))
                    return BadRequest("Invalid content type.");

                var blobId = Guid.NewGuid();
                await using var input = file.OpenReadStream();
                await blobService.AddStream(blobId, input);

                var fontFile = new FontFile
                {
                    BlobId = blobId,
                    Name = file.FileName,
                    Format = extension,
                };

                return new JsonResult(fontFile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while uploading font file.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        });
    }
}