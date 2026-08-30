using Crudspa.Framework.Auth.Server.Services;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Server.Extensions;
using Crudspa.Framework.Core.Shared;
using Crudspa.Framework.Core.Shared.Contracts.Data;
using Microsoft.AspNetCore.SignalR;

namespace Crudspa.Framework.Auth.Server.Filters;

public class SessionAuthHubFilter(
    SessionAuthCache sessions,
    ISessionFetcher sessionFetcher,
    IServerConfigService serverConfigService) : IHubFilter
{
    public async ValueTask<Object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<Object?>> next)
    {
        var principal = invocationContext.Context.User;
        var sessionId = principal?.ReadAuthenticatedSessionId();
        var authPolicyId = principal?.ReadAuthenticatedAuthPolicyId();
        var claimedPortalId = principal?.ReadAuthenticatedPortalId();

        var request = invocationContext.HubMethodArguments.OfType<Request>().FirstOrDefault();
        var legacySessionId = request?.SessionId;
        var checkOnly = invocationContext.HubMethodName == "SessionCheck";

        var portalId = serverConfigService.Fetch().PortalId;
        Boolean valid;

        if (sessionId is not null || claimedPortalId is not null)
            valid = sessionId is not null
                && authPolicyId is not null
                && claimedPortalId == portalId
                && await sessions.Validate(sessionId.Value, authPolicyId.Value, !checkOnly);
        else if (legacySessionId is not null && (await sessionFetcher.Fetch(legacySessionId))?.User?.Id is not null)
        {
            sessionId = legacySessionId;
            valid = await sessions.Validate(sessionId.Value, !checkOnly);
        }
        else
            return await next(invocationContext);

        if (!valid)
        {
            await invocationContext.Hub.Clients.Client(invocationContext.Context.ConnectionId)
                .SendAsync("AuthSessionEnded");
            throw new HubException(Constants.ErrorMessages.PermissionDenied);
        }

        foreach (var argument in invocationContext.HubMethodArguments.OfType<Request>())
            argument.SessionId = sessionId;

        return await next(invocationContext);
    }
}