using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Auth.Server.Sproxies;
using Crudspa.Framework.Auth.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Server.Contracts.Data;

namespace Crudspa.Framework.Auth.Server.Services;

public class SessionAuthServiceSql(IServerConfigService configService) : ISessionAuthService
{
    private String Connection => configService.Fetch().Database;
    private Guid PortalId => configService.Fetch().PortalId;

    public Task<Boolean> Start(Guid sessionId, Guid userId, NativeAuthMethod method)
    {
        var provider = method switch
        {
            NativeAuthMethod.PasswordEmailCode => AuthProviders.PasswordEmailCode,
            NativeAuthMethod.EmailCode => AuthProviders.EmailCode,
            NativeAuthMethod.StudentCode => AuthProviders.StudentCode,
            _ => throw new ArgumentOutOfRangeException(nameof(method)),
        };

        return SessionAuthStart.Execute(Connection, sessionId, userId, provider);
    }

    public Task<SessionAuthState?> Validate(Guid sessionId, Guid portalId, DateTimeOffset lastActivity) =>
        SessionAuthValidate.Execute(Connection, sessionId, portalId, lastActivity);

    public Task<Boolean> Revoke(Guid sessionId, Guid portalId, String reason) =>
        SessionAuthRevoke.Execute(Connection, sessionId, portalId, reason);

    public Task<Boolean> Revoke(Guid sessionId, String reason) =>
        Revoke(sessionId, PortalId, reason);
}