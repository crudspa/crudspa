using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Auth.Server.Extensions;
using Crudspa.Framework.Auth.Server.Services;
using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Crudspa.Framework.Auth.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Shared.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Samples.Auth.Server.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    AuthDestinationService destinations,
    AuthProviderRegistry providers,
    AuthStartPolicyServiceSql policies,
    AuthTransactionServiceSql transactions) : Controller
{
    [HttpGet("start")]
    public async Task<IActionResult> Start([FromQuery] AuthRequest request)
    {
        if (!request.IsValid()
            || String.Equals(request.Audience, "auto", StringComparison.OrdinalIgnoreCase)
            || request.Tenant.HasNothing())
            return BadRequest();

        request.Tenant = request.Tenant!.Trim();
        var policy = await policies.Resolve(null, request.Audience!, request.Tenant);
        var provider = policy?.Provider.HasSomething() == true ? providers.Find(policy.Provider!) : null;

        if (policy?.AutoRedirect != true || provider?.Enabled != true)
            return NotFound();

        var properties = await transactions.Start(request, provider, policy);
        return Challenge(properties, provider.ChallengeScheme);
    }

    [HttpGet("{provider}/start")]
    public async Task<IActionResult> Start(String provider, [FromQuery] AuthRequest request)
    {
        if (!request.IsValid()) return BadRequest();

        var authenticationProvider = providers.Find(provider);
        if (authenticationProvider?.Enabled != true) return NotFound();

        AuthStartPolicy? policy = null;

        if (!String.Equals(request.Audience, "auto", StringComparison.OrdinalIgnoreCase))
        {
            if (request.Tenant.HasNothing()) return BadRequest();

            request.Tenant = request.Tenant!.Trim();
            policy = await policies.Resolve(authenticationProvider.Key, request.Audience!, request.Tenant);
            if (policy is null) return NotFound();
        }

        var properties = await transactions.Start(request, authenticationProvider, policy);
        return Challenge(properties, authenticationProvider.ChallengeScheme);
    }

    [HttpGet("complete")]
    public async Task<IActionResult> Complete()
    {
        foreach (var provider in providers.Enabled)
        {
            var result = await HttpContext.AuthenticateAsync(provider.SessionScheme);
            if (!result.Succeeded) continue;

            await HttpContext.SignOutAsync(provider.SessionScheme);

            var principal = result.Principal;
            var authenticatedProvider = principal?.FindFirst(AuthClaimTypes.Provider)?.Value;
            if (principal is null || !String.Equals(authenticatedProvider, provider.Key, StringComparison.OrdinalIgnoreCase) || result.Properties is null)
                return RedirectToAction(nameof(Error));

            var completion = await transactions.Complete(principal, result.Properties);
            if (completion.Code != AuthCompletion.Codes.Success)
                return RedirectToAction(nameof(Error));

            if (String.IsNullOrWhiteSpace(completion.Audience) || String.IsNullOrWhiteSpace(completion.HandoffCode))
                return RedirectToAction(nameof(Error));

            return Redirect(destinations.Resolve(completion.Audience!, completion.HandoffCode!).ToString());
        }

        return Unauthorized(new { Code = "auth-failed" });
    }

    [HttpGet("error")]
    public IActionResult Error()
    {
        return new ContentResult
        {
            Content = """
                <!doctype html>
                <html lang="en">
                <head><meta charset="utf-8"><meta name="viewport" content="width=device-width"><title>Sign-in unsuccessful</title></head>
                <body><main><h1>We couldn't sign you in</h1><p>Please close this page and try again from Crudspa. If the problem continues, contact your school or district.</p></main></body>
                </html>
                """,
            ContentType = "text/html; charset=utf-8",
            StatusCode = StatusCodes.Status401Unauthorized,
        };
    }
}