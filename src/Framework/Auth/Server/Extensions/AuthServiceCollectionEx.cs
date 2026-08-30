using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Auth.Server.Contracts.Ids;
using Crudspa.Framework.Auth.Server.Filters;
using Crudspa.Framework.Auth.Server.Services;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Server.Extensions;
using Crudspa.Framework.Core.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Crudspa.Framework.Auth.Server.Extensions;

public static class AuthServiceCollectionEx
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new AuthDestinationService(configuration));
        services.AddSingleton<AuthFailureServiceSql>();
        services.AddSingleton<AuthHandoffServiceSql>();
        services.AddSingleton<AuthRouteServiceSql>();
        services.AddSingleton<AuthProviderRegistry>();
        services.AddSingleton<AuthStartPolicyServiceSql>();
        services.AddSingleton<AuthTransactionServiceSql>();
        return services;
    }

    public static IServiceCollection AddPortalAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var config = new SessionAuthConfig(configuration);
        config.Validate();

        services.AddSingleton(config);
        services.AddSingleton<AuthHandoffServiceSql>();
        services.AddSingleton<AuthRouteServiceSql>();
        services.AddSingleton<SessionAuthCache>();
        services.AddSingleton<SessionAuthServiceSql>();
        services.AddSingleton<ISessionAuthService>(provider => provider.GetRequiredService<SessionAuthServiceSql>());
        services.AddSingleton<SessionAuthHubFilter>();
        services.AddAntiforgery(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.Name = config.AntiforgeryCookieName;
            options.Cookie.Path = "/";
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.HeaderName = Constants.HeaderKeys.RequestVerificationToken;
        });
        services.AddAuthorization();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthSchemes.PortalSession;
                options.DefaultSignInScheme = AuthSchemes.PortalSession;
            })
            .AddCookie(AuthSchemes.PortalSession, options =>
            {
                options.Cookie.Name = config.CookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Path = "/";
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = false;
                options.Events = new()
                {
                    OnValidatePrincipal = async context =>
                    {
                        var sessionId = context.Principal?.ReadAuthenticatedSessionId();
                        var authPolicyId = context.Principal?.ReadAuthenticatedAuthPolicyId();
                        var claimedPortalId = context.Principal?.ReadAuthenticatedPortalId();
                        var portalId = context.HttpContext.RequestServices.GetRequiredService<IServerConfigService>().Fetch().PortalId;
                        var sessions = context.HttpContext.RequestServices.GetRequiredService<SessionAuthCache>();

                        if (sessionId is not null
                            && authPolicyId is not null
                            && claimedPortalId == portalId
                            && await sessions.Validate(sessionId.Value, authPolicyId.Value))
                            return;

                        context.RejectPrincipal();
                        DeleteCookies(context.HttpContext, config);
                    },
                };
            });

        return services;
    }

    public static IApplicationBuilder UsePortalAuth(this IApplicationBuilder app)
    {
        var config = app.ApplicationServices.GetRequiredService<SessionAuthConfig>();

        app.UseAuthentication();
        app.Use(async (context, next) =>
        {
            var path = context.Request.Path;
            var securedTransport = path.StartsWithSegments("/api") || path.StartsWithSegments("/hub");
            var authenticatedSessionId = context.User.ReadAuthenticatedSessionId();

            if (securedTransport
                && context.Request.Cookies.ContainsKey(config.CookieName)
                && authenticatedSessionId is null)
            {
                DeleteCookies(context, config);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            if (securedTransport && authenticatedSessionId is null)
            {
                var key = Constants.CookieKeys.Resolve(Constants.CookieKeys.SessionId, context.Request.Host.Host, context.Request.Host.Port);
                var value = context.Request.Cookies[key];

                if (Guid.TryParse(value, out var legacySessionId))
                {
                    var session = await context.RequestServices.GetRequiredService<ISessionFetcher>().Fetch(legacySessionId);
                    if (session?.User?.Id is not null
                        && !await context.RequestServices.GetRequiredService<SessionAuthCache>().Validate(legacySessionId))
                    {
                        DeleteCookies(context, config);
                        context.Response.Cookies.Delete(key, new()
                        {
                            HttpOnly = false,
                            Path = "/",
                            SameSite = SameSiteMode.Lax,
                            Secure = true,
                        });
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                }
            }

            await next();
        });

        return app;
    }

    private static void DeleteCookies(HttpContext context, SessionAuthConfig config)
    {
        DeleteCookie(context, config.CookieName, SameSiteMode.Lax);
        DeleteCookie(context, config.AntiforgeryCookieName, SameSiteMode.Strict);
    }

    private static void DeleteCookie(HttpContext context, String name, SameSiteMode sameSite)
    {
        context.Response.Cookies.Delete(name, new()
        {
            HttpOnly = true,
            Path = "/",
            SameSite = sameSite,
            Secure = true,
        });
    }
}