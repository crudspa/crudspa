using Crudspa.Framework.Auth.Server.Extensions;
using Crudspa.Framework.Auth.Server.Sproxies;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Microsoft.AspNetCore.Authentication;

namespace Crudspa.Framework.Auth.Server.Services;

public class AuthFailureServiceSql(IServerConfigService configService)
{
    public Task Record(AuthenticationProperties? properties, String provider, Exception? failure)
    {
        var correlationId = properties?.Items.TryGetValue(AuthRequestEx.TransactionProperty, out var transactionValue) == true
            && Guid.TryParse(transactionValue, out var transactionId)
                ? transactionId
                : Guid.NewGuid();
        String? audience = null;
        properties?.Items.TryGetValue(AuthRequestEx.AudienceProperty, out audience);
        var reason = Reason(failure);

        return AuthFailureInsert.Execute(configService.Fetch().Database, correlationId, provider, audience, reason);
    }

    private static String Reason(Exception? failure)
    {
        var exception = failure?.GetBaseException();
        if (exception == null) return "RemoteFailure";

        var message = exception.Message;
        if (message.Contains("invalid_client", StringComparison.OrdinalIgnoreCase)) return "OidcInvalidClient";
        if (message.Contains("invalid_grant", StringComparison.OrdinalIgnoreCase)) return "OidcInvalidGrant";
        if (message.Contains("IDX21336", StringComparison.OrdinalIgnoreCase)) return "OidcMissingIdToken";
        return exception.GetType().Name;
    }
}