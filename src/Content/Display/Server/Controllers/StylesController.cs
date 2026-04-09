using System.Text;
using Crudspa.Framework.Core.Server.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Content.Display.Server.Controllers;

[Route("api/content/display/styles")]
public class StylesController(
    ILogger<StylesController> logger,
    IHostEnvironment hostingEnvironment,
    IThemeRunService themeRunService)
    : ControllerBase
{
    [HttpGet("fetch")]
    public async Task<ActionResult> Fetch(Guid? portal, Int32 revision, String build)
    {
        try
        {
            if (!TryPortal(portal, out var portalId))
                return NotFound();

            if (!hostingEnvironment.IsDevelopment())
            {
                var currentRevision = await themeRunService.FetchRevision(portalId);

                if (!currentRevision.HasValue)
                    return NotFound();

                if (!currentRevision.Value.Equals(revision))
                    return RedirectToAction(nameof(Fetch), new
                    {
                        portal = portalId,
                        revision = currentRevision.Value,
                        build,
                    })!;
            }

            var css = await themeRunService.Fetch(portalId, revision, build);

            if (css.HasNothing())
                return NotFound();

            Response.SetCacheHeaders(false);

            return File(Encoding.UTF8.GetBytes(css), CssPaths.Type);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while fetching portal styles. Portal: {portal} Revision: {revision} Build: {build}", portal, revision, build);
            return NotFound();
        }
    }

    [HttpGet("preview")]
    public async Task<ActionResult> Preview(Guid? portal, String scope, Int32 version = 0)
    {
        try
        {
            if (!TryPortal(portal, out var portalId) || scope.HasNothing() || !scope.IsSimpleKey())
                return NotFound();

            Response.SetCacheHeaders(!hostingEnvironment.IsDevelopment() && version > 0);

            var css = await themeRunService.Preview(portalId, scope);

            if (css.HasNothing())
                return NotFound();

            return File(Encoding.UTF8.GetBytes(css), CssPaths.Type);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while fetching preview portal styles. PortalId: {portal} Scope: {scope} Version: {version}", portal, scope, version);
            return NotFound();
        }
    }

    private static Boolean TryPortal(Guid? portal, out Guid portalId)
    {
        portalId = portal.GetValueOrDefault();
        return portal.HasValue && !portalId.Equals(Guid.Empty);
    }
}