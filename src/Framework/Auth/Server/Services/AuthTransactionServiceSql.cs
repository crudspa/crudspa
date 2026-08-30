using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Auth.Server.Extensions;
using Crudspa.Framework.Auth.Server.Sproxies;
using Crudspa.Framework.Auth.Shared.Contracts.Behavior;
using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Crudspa.Framework.Auth.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Extensions;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Crudspa.Framework.Auth.Server.Services;

public class AuthTransactionServiceSql(
    IServerConfigService configService,
    ICryptographyService cryptographyService)
{
    private String Connection => configService.Fetch().Database;

    public async Task<AuthenticationProperties> Start(AuthRequest request, IAuthProvider provider, AuthStartPolicy? policy = null)
    {
        var id = cryptographyService.GetRandomGuid();
        var audience = request.Audience!.ToLowerInvariant();
        var providerKey = provider.Key.ToLowerInvariant();
        var returnPath = request.ReturnPath.HasSomething() ? request.ReturnPath! : "/";

        await AuthTransactionInsert.Execute(Connection, id, providerKey, audience, returnPath);
        return request.ToProperties(providerKey, id, policy);
    }

    public async Task<AuthCompletion> Complete(ClaimsPrincipal principal, AuthenticationProperties properties)
    {
        if (!properties.Items.TryGetValue(AuthRequestEx.TransactionProperty, out var transactionValue)
            || !Guid.TryParse(transactionValue, out var transactionId))
            return InvalidIdentity();

        var provider = principal.FindFirst(AuthClaimTypes.Provider)?.Value;
        var issuer = principal.FindFirst(AuthClaimTypes.Issuer)?.Value;
        var subject = principal.FindFirst(AuthClaimTypes.Subject)?.Value;
        var tenant = principal.FindFirst(AuthClaimTypes.Tenant)?.Value;
        var role = principal.FindFirst(AuthClaimTypes.Role)?.Value;
        var audience = principal.FindFirst(AuthClaimTypes.Audience)?.Value;

        if (provider.HasNothing() || issuer.HasNothing() || subject.HasNothing() || tenant.HasNothing() || audience.HasNothing())
            return InvalidIdentity();

        if (properties.Items.TryGetValue(AuthRequestEx.TenantProperty, out var trustedTenant)
            && trustedTenant.HasSomething()
            && !String.Equals(trustedTenant, tenant, StringComparison.Ordinal))
            return InvalidIdentity();

        provider = provider!.ToLowerInvariant();
        var handoffCode = cryptographyService.GetRandomGuid().ToString("N");
        var completion = await AuthComplete.Execute(
            Connection,
            transactionId,
            provider,
            issuer!,
            subject!,
            tenant!,
            role,
            audience!,
            cryptographyService.ComputeIdentityKey(provider, issuer!, subject!),
            cryptographyService.GetRandomGuid(),
            cryptographyService.ComputeHash(handoffCode));

        if (completion.Code == AuthCompletion.Codes.Success)
            completion.HandoffCode = handoffCode;

        return completion;
    }

    private static AuthCompletion InvalidIdentity()
    {
        return new() { Code = AuthCompletion.Codes.InvalidIdentity };
    }
}