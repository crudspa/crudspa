using Crudspa.Framework.Core.Server.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Content.Display.Server.Controllers;

[Route("api/content/display/images")]
public class ImagesController : EmbeddedResourceController
{
    private const String Namespace = "Crudspa.Content.Display.Server.Embedded.Images.";

    private static readonly EmbeddedResourceFileCache Files = new(typeof(ImagesController).Assembly, Namespace);

    [HttpGet("check-empty")]
    public ActionResult CheckEmpty() => GetFile("Check-Empty.svg");

    [HttpGet("check-filled")]
    public ActionResult CheckFilled() => GetFile("Check-Filled.svg");

    [HttpGet("error")]
    public ActionResult Error() => GetFile("Error.svg");

    [HttpGet("exclamation")]
    public ActionResult Exclamation() => GetFile("Exclamation.svg");

    [HttpGet("notebook")]
    public ActionResult Notebook() => GetFile("Notebook.svg");

    [HttpGet("star-empty")]
    public ActionResult StarEmpty() => GetFile("Star-Empty.svg");

    [HttpGet("star-filled")]
    public ActionResult StarFilled() => GetFile("Star-Filled.svg");

    private ActionResult GetFile(String fileName) => EmbeddedResourceFile(Files, fileName);
}