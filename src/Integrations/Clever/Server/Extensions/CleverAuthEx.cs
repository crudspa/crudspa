using Crudspa.Framework.Auth.Server.Extensions;
using Crudspa.Framework.Auth.Server.Services;
using Crudspa.Framework.Auth.Shared.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using Crudspa.Integrations.Clever.Server.Contracts.Data;
using Crudspa.Integrations.Clever.Server.Contracts.Ids;
using Crudspa.Integrations.Clever.Server.Services;

namespace Crudspa.Integrations.Clever.Server.Extensions;

public static class CleverAuthEx
{
    public static IServiceCollection AddCleverAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var config = new CleverConfig(configuration);
        services.AddSingleton<IAuthProvider>(new CleverAuthProvider(config.Enabled));

        if (!config.Enabled) return services;
        if (config.ClientId.HasNothing() || config.ClientSecret.HasNothing())
            throw new InvalidOperationException("Clever authentication is enabled, but its credentials are not configured.");

        services.AddSingleton(config);
        services.AddHttpClient<CleverClient>();
        services.AddAuthentication()
            .AddCookie(CleverAuthSchemes.Session, options =>
            {
                options.Cookie.Name = "__Host-Crudspa.Auth.Clever";
                options.Cookie.HttpOnly = true;
                options.Cookie.Path = "/";
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.SlidingExpiration = false;
            })
            .AddOAuth<OAuthOptions, CleverAuthHandler>(CleverAuthSchemes.Challenge, options =>
            {
                options.AuthorizationEndpoint = "https://clever.com/oauth/authorize";
                options.TokenEndpoint = "https://clever.com/oauth/tokens";
                options.UserInformationEndpoint = "https://api.clever.com/userinfo";
                options.ClientId = config.ClientId!;
                options.ClientSecret = config.ClientSecret!;
                options.CallbackPath = "/signin-clever";
                options.SignInScheme = CleverAuthSchemes.Session;
                options.SaveTokens = false;
                options.UsePkce = false;
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.CorrelationCookie.Name = "__Host-Crudspa.Auth.Clever.Correlation.";
                options.CorrelationCookie.HttpOnly = true;
                options.CorrelationCookie.Path = "/";
                options.CorrelationCookie.SameSite = SameSiteMode.None;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Events = new()
                {
                    OnRedirectToAuthorizationEndpoint = context =>
                    {
                        if (context.Properties.Items.TryGetValue(AuthRequestEx.AudienceProperty, out var audience)
                            && String.Equals(audience, "student", StringComparison.OrdinalIgnoreCase))
                            context.RedirectUri = QueryHelpers.AddQueryString(context.RedirectUri, "confirmed", "false");

                        if (context.Properties.Items.TryGetValue(AuthRequestEx.TenantProperty, out var tenant)
                            && tenant.HasSomething())
                            context.RedirectUri = QueryHelpers.AddQueryString(context.RedirectUri, "district_id", tenant!);

                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnCreatingTicket = async context =>
                    {
                        var client = context.HttpContext.RequestServices.GetRequiredService<CleverClient>();
                        var value = await client.FetchIdentity(context.AccessToken!, context.HttpContext.RequestAborted);
                        var identity = context.Identity!;
                        Add(identity, "iss", "https://clever.com");
                        Add(identity, "sub", value.Sub);
                        Add(identity, "district_id", value.DistrictId);
                        Add(identity, "user_type", value.UserType);
                        Add(identity, "authorized_by", value.AuthorizedBy);
                    },
                    OnRemoteFailure = async context =>
                    {
                        await context.HttpContext.RequestServices
                            .GetRequiredService<AuthFailureServiceSql>()
                            .Record(context.Properties, CleverAuthSchemes.Provider, context.Failure);
                        context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger(CleverAuthSchemes.Provider)
                            .LogWarning(context.Failure, "Clever authentication failed.");
                        var fallback = context.Properties?.Fallback(
                            context.HttpContext.RequestServices.GetRequiredService<AuthDestinationService>());
                        context.Response.Redirect(fallback?.ToString() ?? "/auth/error");
                        context.HandleResponse();
                    },
                    OnTicketReceived = context =>
                    {
                        if (!CleverIdentityEx.Normalize(context.Principal))
                            context.Fail("Clever returned an incomplete or unauthorized identity.");

                        return Task.CompletedTask;
                    },
                };
            });

        return services;
    }

    private static void Add(ClaimsIdentity identity, String type, String? value)
    {
        if (value.HasSomething()) identity.AddClaim(new(type, value));
    }
}