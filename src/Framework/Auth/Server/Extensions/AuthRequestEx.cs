using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Auth.Server.Services;
using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Crudspa.Framework.Core.Shared.Extensions;
using Microsoft.AspNetCore.Authentication;

namespace Crudspa.Framework.Auth.Server.Extensions;

public static class AuthRequestEx
{
    public const String AudienceProperty = "audience";
    public const String ProviderProperty = "provider";
    public const String TenantProperty = "tenant";
    public const String FallbackProperty = "fallback";
    public const String ReturnPathProperty = "return-path";
    public const String TransactionProperty = "transaction";

    public static Boolean IsValid(this AuthRequest request)
    {
        return IsValidAudience(request.Audience)
            && IsValidTenant(request.Tenant)
            && IsValidReturnPath(request.ReturnPath);
    }

    public static AuthenticationProperties ToProperties(
        this AuthRequest request,
        String provider,
        Guid transactionId,
        AuthStartPolicy? policy)
    {
        return new()
        {
            AllowRefresh = false,
            IsPersistent = false,
            RedirectUri = "/auth/complete",
            Items =
            {
                [AudienceProperty] = request.Audience!.ToLowerInvariant(),
                [ProviderProperty] = provider,
                [TenantProperty] = policy?.Tenant,
                [FallbackProperty] = policy?.Fallback == true ? Boolean.TrueString : Boolean.FalseString,
                [ReturnPathProperty] = request.ReturnPath.HasSomething() ? request.ReturnPath : "/",
                [TransactionProperty] = transactionId.ToString("D"),
            },
        };
    }

    public static Uri? Fallback(this AuthenticationProperties properties, AuthDestinationService destinations)
    {
        if (!properties.Items.TryGetValue(FallbackProperty, out var fallback)
            || !Boolean.TryParse(fallback, out var enabled)
            || !enabled
            || !properties.Items.TryGetValue(AudienceProperty, out var audience)
            || audience.HasNothing()
            || !properties.Items.TryGetValue(ReturnPathProperty, out var returnPath))
            return null;

        return destinations.ResolveFallback(audience!, returnPath.HasSomething() ? returnPath! : "/");
    }

    private static Boolean IsValidAudience(String? audience)
    {
        return String.Equals(audience, "auto", StringComparison.OrdinalIgnoreCase)
            || String.Equals(audience, "district", StringComparison.OrdinalIgnoreCase)
            || String.Equals(audience, "school", StringComparison.OrdinalIgnoreCase)
            || String.Equals(audience, "student", StringComparison.OrdinalIgnoreCase);
    }

    private static Boolean IsValidReturnPath(String? returnPath)
    {
        if (returnPath.HasNothing()) return true;

        return returnPath.StartsWith('/')
            && !returnPath.StartsWith("//", StringComparison.Ordinal)
            && !returnPath.Contains('\\')
            && !returnPath.Any(Char.IsControl)
            && Uri.TryCreate(returnPath, UriKind.Relative, out _);
    }

    private static Boolean IsValidTenant(String? tenant)
    {
        return tenant.HasNothing()
            || (tenant!.Length <= 255 && !tenant.Any(Char.IsControl));
    }
}