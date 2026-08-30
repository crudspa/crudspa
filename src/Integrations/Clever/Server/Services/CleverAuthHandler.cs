using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Crudspa.Integrations.Clever.Server.Services;

public class CleverAuthHandler(
    IOptionsMonitor<OAuthOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    CleverClient client) : OAuthHandler<OAuthOptions>(options, logger, encoder)
{
    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        if (Request.Query.ContainsKey("code") && !Request.Query.ContainsKey("state"))
        {
            Response.Redirect("/auth/clever/start?audience=auto");
            return HandleRequestResult.Handle();
        }

        return await base.HandleRemoteAuthenticateAsync();
    }

    protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context) =>
        OAuthTokenResponse.Success(await client.Redeem(context.Code, context.RedirectUri, Context.RequestAborted));
}