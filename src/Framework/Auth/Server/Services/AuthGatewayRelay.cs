using Azure.Messaging.EventGrid;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Contracts.Data;
using Crudspa.Framework.Core.Shared.Contracts.Events;
using Crudspa.Framework.Core.Shared.Extensions;

namespace Crudspa.Framework.Auth.Server.Services;

public static class AuthGatewayRelay
{
    private const String Subject = "Crudspa.Framework.Core.Shared.Contracts.Events.AuthSessionsRevoked";

    public static Boolean TryHandle(
        EventGridEvent gridEvent,
        ISessionFetcher sessionFetcher,
        SessionAuthCache? sessionAuthCache = null)
    {
        if (gridEvent.Subject != Subject)
            return false;

        var payload = gridEvent.Data.ToString().FromJson<AuthSessionsRevoked>();

        if (payload?.PolicyIds.HasItems() == true)
        {
            sessionFetcher.InvalidateAll();
            sessionAuthCache?.Invalidate(payload.PolicyIds);
        }

        return true;
    }
}