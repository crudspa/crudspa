using Crudspa.Framework.Auth.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Shared.Extensions;
using Crudspa.Integrations.Clever.Server.Contracts.Ids;
using System.Security.Claims;

namespace Crudspa.Integrations.Clever.Server.Services;

public static class CleverIdentityEx
{
    public static Boolean Normalize(ClaimsPrincipal? principal)
    {
        if (principal?.Identity is not ClaimsIdentity identity) return false;

        var issuer = principal.FindFirst("iss")?.Value;
        var subject = principal.FindFirst("sub")?.Value;
        var district = principal.FindFirst("district_id")?.Value ?? principal.FindFirst("district")?.Value;
        var authorizedBy = principal.FindFirst("authorized_by")?.Value;
        var role = NormalizeRole(principal.FindFirst("user_type")?.Value);
        var audience = AudienceFor(role);

        if (issuer.HasNothing() || subject.HasNothing() || district.HasNothing() || role.HasNothing() || audience.HasNothing()
            || !String.Equals(authorizedBy, "district", StringComparison.OrdinalIgnoreCase))
            return false;

        foreach (var claim in identity.Claims.ToList())
            identity.RemoveClaim(claim);

        identity.AddClaim(new(AuthClaimTypes.Audience, audience));
        identity.AddClaim(new(AuthClaimTypes.Issuer, issuer));
        identity.AddClaim(new(AuthClaimTypes.Provider, CleverAuthSchemes.Provider));
        identity.AddClaim(new(AuthClaimTypes.Role, role));
        identity.AddClaim(new(AuthClaimTypes.Subject, subject));
        identity.AddClaim(new(AuthClaimTypes.Tenant, district));
        return true;
    }

    private static String? AudienceFor(String? role)
    {
        if (String.Equals(role, "student", StringComparison.Ordinal)) return "student";
        if (String.Equals(role, "teacher", StringComparison.Ordinal)) return "school";
        if (String.Equals(role, "staff", StringComparison.Ordinal)) return "school";
        if (String.Equals(role, "district_admin", StringComparison.Ordinal)) return "district";
        return null;
    }

    private static String? NormalizeRole(String? role)
    {
        if (String.Equals(role, "student", StringComparison.OrdinalIgnoreCase)) return "student";
        if (String.Equals(role, "teacher", StringComparison.OrdinalIgnoreCase)) return "teacher";
        if (String.Equals(role, "staff", StringComparison.OrdinalIgnoreCase)) return "staff";
        if (String.Equals(role, "district admin", StringComparison.OrdinalIgnoreCase)
            || String.Equals(role, "district_admin", StringComparison.OrdinalIgnoreCase)) return "district_admin";
        return null;
    }
}