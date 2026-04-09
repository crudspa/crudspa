using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Framework.Core.Server.Controllers;

[Route("api/framework/core/pdf-file")]
public class PdfFileController(
    ILogger<PdfFileController> logger,
    IControllerWrappers controllerWrappers,
    IBlobService blobService,
    IPdfFileService pdfFileService)
    : ControllerBase
{
    [HttpGet("fetch")]
    public async Task<ActionResult> Fetch(Guid? id, Guid? version, Boolean download = false, Boolean original = false)
    {
        return await controllerWrappers.RequireSession(Request, async session =>
        {
            try
            {
                var response = await pdfFileService.FetchAndLog(new(session.Id, new() { Id = id }));

                if (!response.Ok)
                {
                    logger.LogWarning("PDF file not found. Id: {id}", id);
                    return NotFound();
                }

                var pdfFile = response.Value;

                var stream = await blobService.TryFetchStream(pdfFile.BlobId.GetValueOrDefault());

                if (stream is null)
                {
                    logger.LogWarning("Blob for PDF file not found. Id: {id}", id);
                    return NotFound();
                }

                Response.SetCacheHeaders(true);

                var mime = pdfFile.Format.ToMimeType();

                return download
                    ? File(stream, mime, fileDownloadName: pdfFile.Name, enableRangeProcessing: true)
                    : File(stream, mime, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while fetching PDF file. Id: {id}", id);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
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

                if (!Constants.AllowedPdfExtensions.HasAny(x => x.IsBasically(extension)))
                    return BadRequest("Invalid file type. Only PDF files are allowed.");

                if (!Constants.AllowedPdfContentTypes.HasAny(x => x.IsBasically(file.ContentType)))
                    return BadRequest("Invalid content type.");

                var blobId = Guid.NewGuid();
                await using var input = file.OpenReadStream();
                await blobService.AddStream(blobId, input);

                var pdfFile = new PdfFile
                {
                    BlobId = blobId,
                    Name = file.FileName,
                    Format = extension,
                };

                return new JsonResult(pdfFile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while uploading PDF file.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        });
    }
}