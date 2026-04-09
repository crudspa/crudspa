using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Content.Display.Server.Controllers;

[Route("api/content/display/sounds")]
public class SoundsController : ControllerBase
{
    private const String Namespace = "Crudspa.Content.Display.Server.Embedded.Sounds.";

    private readonly Assembly _assembly = typeof(SoundsController).Assembly;
    private readonly Dictionary<String, Byte[]> _sounds = new();

    public SoundsController(IEmbeddedResourceService embeddedResourceService)
    {
        _sounds["achievement-grow"] = embeddedResourceService.ReadBytesNow(_assembly, $"{Namespace}AchievementGrow.mp3");
        _sounds["achievement-shrink"] = embeddedResourceService.ReadBytesNow(_assembly, $"{Namespace}AchievementShrink.mp3");
        _sounds["button-press"] = embeddedResourceService.ReadBytesNow(_assembly, $"{Namespace}ButtonPress.mp3");
        _sounds["choice-correct"] = embeddedResourceService.ReadBytesNow(_assembly, $"{Namespace}ChoiceCorrect.mp3");
        _sounds["choice-incorrect"] = embeddedResourceService.ReadBytesNow(_assembly, $"{Namespace}ChoiceIncorrect.mp3");
        _sounds["choice-selected"] = embeddedResourceService.ReadBytesNow(_assembly, $"{Namespace}ChoiceSelected.mp3");
        _sounds["tada"] = embeddedResourceService.ReadBytesNow(_assembly, $"{Namespace}Tada.mp3");
    }

    [HttpGet("achievement-grow.mp3")]
    public ActionResult AchievementGrow()
    {
        const String fileName = "achievement-grow.mp3";
        SetHeaders(fileName);
        return File(_sounds["achievement-grow"], "audio/mpeg", fileName);
    }

    [HttpGet("achievement-shrink.mp3")]
    public ActionResult AchievementShrink()
    {
        const String fileName = "achievement-shrink.mp3";
        SetHeaders(fileName);
        return File(_sounds["achievement-shrink"], "audio/mpeg", fileName);
    }

    [HttpGet("button-press.mp3")]
    public ActionResult ButtonPress()
    {
        const String fileName = "button-press.mp3";
        SetHeaders(fileName);
        return File(_sounds["button-press"], "audio/mpeg", fileName);
    }

    [HttpGet("choice-correct.mp3")]
    public ActionResult ChoiceCorrect()
    {
        const String fileName = "choice-correct.mp3";
        SetHeaders(fileName);
        return File(_sounds["choice-correct"], "audio/mpeg", fileName);
    }

    [HttpGet("choice-incorrect.mp3")]
    public ActionResult ChoiceIncorrect()
    {
        const String fileName = "choice-incorrect.mp3";
        SetHeaders(fileName);
        return File(_sounds["choice-incorrect"], "audio/mpeg", fileName);
    }

    [HttpGet("choice-selected.mp3")]
    public ActionResult ChoiceSelected()
    {
        const String fileName = "choice-selected.mp3";
        SetHeaders(fileName);
        return File(_sounds["choice-selected"], "audio/mpeg", fileName);
    }

    [HttpGet("tada.mp3")]
    public ActionResult Tada()
    {
        const String fileName = "tada.mp3";
        SetHeaders(fileName);
        return File(_sounds["tada"], "audio/mpeg", fileName);
    }

    private void SetHeaders(String fileName)
    {
        var headers = Response.GetTypedHeaders();
        headers.CacheControl = HttpResponseEx.NeverChanges;
        headers.ContentDisposition = new("inline") { FileNameStar = fileName };
    }
}