using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Auth.Server.Contracts.Ids;
using Crudspa.Framework.Auth.Server.Services;
using Crudspa.Framework.Auth.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Server.Extensions;
using Crudspa.Framework.Core.Shared;
using Crudspa.Framework.Core.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Shared.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Crudspa.Framework.Auth.Server.Controllers;

[ApiController]
[Route("auth")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class PortalAuthController(
    AuthHandoffServiceSql handoffs,
    AuthRouteServiceSql routes,
    SessionAuthServiceSql sessions,
    SessionAuthCache sessionCache,
    SessionAuthConfig config,
    IServerConfigService serverConfigService,
    IConfiguration configuration,
    IAntiforgery antiforgery,
    ILogger<PortalAuthController> logger) : Controller
{
    [HttpGet("{provider}/start")]
    public IActionResult Start(String provider)
    {
        if (provider.HasNothing()
            || provider.Length > 75
            || provider.Any(character => !Char.IsLetterOrDigit(character) && character != '-'))
            return BadRequest();

        var authUrl = configuration["Crudspa.Framework.Core.Server.AuthUrl"];
        if (!Uri.TryCreate(authUrl, UriKind.Absolute, out var authority)
            || authority.Scheme != Uri.UriSchemeHttps
            || authority.UserInfo.HasSomething()
            || authority.Query.HasSomething()
            || authority.Fragment.HasSomething())
            return NotFound();

        var path = $"auth/{Uri.EscapeDataString(provider)}/start";
        var query = QueryString.Create(new KeyValuePair<String, String?>[]
        {
            new("audience", "auto"),
            new("returnPath", "/"),
        });

        return Redirect(new Uri(authority, path + query).ToString());
    }

    [HttpGet("district/{key}/start")]
    public async Task<IActionResult> StartDistrict(String key)
    {
        if (key.HasNothing()
            || key.Length > 75
            || key.Any(character => !Char.IsAsciiLetterLower(character) && !Char.IsDigit(character) && character != '-'))
            return NotFound();

        var route = await routes.Find(AuthAudiences.Student, key);

        if (route is null || !AuthProviders.IsExternal(route.Provider) || route.Tenant.HasNothing())
            return NotFound();

        var authUrl = configuration["Crudspa.Framework.Core.Server.AuthUrl"];
        if (!Uri.TryCreate(authUrl, UriKind.Absolute, out var authority)
            || authority.Scheme != Uri.UriSchemeHttps
            || authority.UserInfo.HasSomething()
            || authority.Query.HasSomething()
            || authority.Fragment.HasSomething())
            return NotFound();

        var path = $"auth/{Uri.EscapeDataString(route.Provider)}/start";
        var query = QueryString.Create(new KeyValuePair<String, String?>[]
        {
            new("audience", route.Audience),
            new("tenant", route.Tenant),
            new("returnPath", "/"),
        });

        return Redirect(new Uri(authority, path + query).ToString());
    }

    [HttpGet("complete")]
    public async Task<IActionResult> Complete([FromQuery] String? code)
    {
        Response.Headers.Append("Referrer-Policy", "no-referrer");

        if (!Guid.TryParseExact(code, "N", out _))
            return Unauthorized();

        var portalId = serverConfigService.Fetch().PortalId;
        var previousSessionId = User.ReadAuthenticatedSessionId();
        var redemption = await handoffs.Redeem(code, portalId, previousSessionId);

        if (redemption?.SessionId is null
            || redemption.AuthPolicyId is null
            || redemption.AbsoluteExpires is null
            || !Url.IsLocalUrl(redemption.ReturnPath))
            return Unauthorized();

        var identity = new ClaimsIdentity(AuthSchemes.PortalSession);
        identity.AddClaim(new(SessionClaimTypes.SessionId, redemption.SessionId.Value.ToString("D")));
        identity.AddClaim(new(SessionClaimTypes.PortalId, portalId.ToString("D")));
        identity.AddClaim(new(SessionClaimTypes.AuthPolicyId, redemption.AuthPolicyId.Value.ToString("D")));

        await HttpContext.SignInAsync(
            AuthSchemes.PortalSession,
            new ClaimsPrincipal(identity),
            new()
            {
                AllowRefresh = false,
                ExpiresUtc = redemption.AbsoluteExpires,
                IsPersistent = redemption.Persist,
            });

        DeleteCookie(Constants.CookieKeys.Resolve(Constants.CookieKeys.SessionId, Request.Host.Host, Request.Host.Port), false, SameSiteMode.Lax);

        return LocalRedirect(redemption.ReturnPath!);
    }

    [HttpGet("request-token")]
    public IActionResult RequestToken()
    {
        if (User.ReadAuthenticatedSessionId() is null)
            return Unauthorized();

        var token = antiforgery.GetAndStoreTokens(HttpContext).RequestToken;

        if (token.HasNothing())
            return StatusCode(StatusCodes.Status500InternalServerError);

        Response.Headers[Constants.HeaderKeys.RequestVerificationToken] = token;
        return NoContent();
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutSession()
    {
        try
        {
            await antiforgery.ValidateRequestAsync(HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            return BadRequest();
        }

        var sessionId = User.ReadAuthenticatedSessionId();
        var portalId = serverConfigService.Fetch().PortalId;

        if (sessionId is null || !await sessions.Revoke(sessionId.Value, portalId, "signed-out"))
            return Unauthorized();

        sessionCache.Invalidate(sessionId.Value);
        await HttpContext.SignOutAsync(AuthSchemes.PortalSession);
        DeleteCookie(config.AntiforgeryCookieName, true, SameSiteMode.Strict);
        DeleteCookie(Constants.CookieKeys.Resolve(Constants.CookieKeys.SessionId, Request.Host.Host, Request.Host.Port), false, SameSiteMode.Lax);

        logger.LogInformation("Secure session signed out for PortalId: {PortalId}", portalId);
        return NoContent();
    }

    private void DeleteCookie(String name, Boolean httpOnly, SameSiteMode sameSite)
    {
        Response.Cookies.Delete(name, new()
        {
            HttpOnly = httpOnly,
            Path = "/",
            SameSite = sameSite,
            Secure = true,
        });
    }
}