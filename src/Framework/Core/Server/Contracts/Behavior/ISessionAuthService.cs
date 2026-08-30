namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISessionAuthService
{
    Task<Boolean> Start(Guid sessionId, Guid userId, NativeAuthMethod method);
    Task<Boolean> Revoke(Guid sessionId, String reason);
}