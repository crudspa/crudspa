namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface INativeAuthPolicy
{
    Task<NativeAuthMethod?> Resolve(Guid userId);
    Task<ExternalAuthRoute?> ResolveExternal(Guid userId);
}