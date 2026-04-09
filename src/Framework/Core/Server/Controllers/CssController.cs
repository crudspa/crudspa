using System.Text;
using Crudspa.Framework.Core.Server.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Framework.Core.Server.Controllers;

[Route("api/framework/core/css")]
public class CssController(
    ILogger<CssController> logger,
    IHostEnvironment hostingEnvironment,
    ICssService cssService)
    : ControllerBase
{
    [HttpGet("fetch")]
    public async Task<ActionResult> Fetch(String build)
    {
        try
        {
            var css = await cssService.Fetch(build);

            if (css.HasNothing())
                return NotFound();

            Response.SetCacheHeaders(!hostingEnvironment.IsDevelopment());

            return File(Encoding.UTF8.GetBytes(css), CssPaths.Type);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while fetching CSS. Build: {build}", build);
            return NotFound();
        }
    }
}