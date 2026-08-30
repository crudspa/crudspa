using Crudspa.Framework.Core.Shared.Contracts.Ids;
using System.Security.Claims;

namespace Crudspa.Framework.Core.Server.Extensions;

public static class SessionClaimsEx
{
    extension(ClaimsPrincipal principal)
    {
        public Guid? ReadAuthenticatedSessionId()
        {
            return ReadGuid(principal, SessionClaimTypes.SessionId);
        }

        public Guid? ReadAuthenticatedAuthPolicyId()
        {
            return ReadGuid(principal, SessionClaimTypes.AuthPolicyId);
        }

        public Guid? ReadAuthenticatedPortalId()
        {
            return ReadGuid(principal, SessionClaimTypes.PortalId);
        }
    }

    private static Guid? ReadGuid(ClaimsPrincipal principal, String claimType)
    {
        if (principal.Identity?.IsAuthenticated != true)
            return null;

        var value = principal.FindFirst(claimType)?.Value;
        return Guid.TryParse(value, out var parsed) ? parsed : null;
    }
}