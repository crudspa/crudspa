using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Content.Display.Server.Controllers;

[Route("api/content/display/images")]
public class ImagesController(IEmbeddedResourceService embeddedResourceService) : ControllerBase
{
    private readonly Assembly _assembly = typeof(ImagesController).Assembly;
    private readonly Dictionary<String, Byte[]> _cache = new();

    private const String Namespace = "Crudspa.Content.Display.Server.Embedded.Images.";

    [HttpGet("check-empty")]
    public async Task<ActionResult> CheckEmpty() => (await GetFile("Check-Empty")).ActionResult;

    [HttpGet("check-filled")]
    public async Task<ActionResult> CheckFilled() => (await GetFile("Check-Filled")).ActionResult;

    [HttpGet("error")]
    public async Task<ActionResult> Error() => (await GetFile("Error")).ActionResult;

    [HttpGet("exclamation")]
    public async Task<ActionResult> Exclamation() => (await GetFile("Exclamation")).ActionResult;

    [HttpGet("notebook")]
    public async Task<ActionResult> Notebook() => (await GetFile("Notebook")).ActionResult;

    private async Task<FileResultWrapper> GetFile(String file)
    {
        const String extension = ".svg";
        var fileName = $"{file}{extension}";

        if (!_cache.ContainsKey(file))
            _cache[file] = await embeddedResourceService.ReadBytes(_assembly, $"{Namespace}{fileName}");

        var actionResult = File(_cache[file], extension.ToMimeType(), $"{fileName}");

        var headers = Response.GetTypedHeaders();

        headers.CacheControl = HttpResponseEx.NeverChanges;
        headers.ContentDisposition = new("inline") { FileNameStar = $"{fileName}" };

        return new()
        {
            ActionResult = actionResult,
            Bytes = _cache[file],
        };
    }
}

public class FileResultWrapper
{
    public ActionResult ActionResult { get; set; } = null!;
    public Byte[] Bytes { get; set; } = null!;
}