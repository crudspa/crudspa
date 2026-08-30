using Crudspa.Framework.Auth.Server.Sproxies;
using Crudspa.Framework.Auth.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Server.Contracts.Data;
using Crudspa.Framework.Core.Shared.Contracts.Behavior;

namespace Crudspa.Framework.Auth.Server.Services;

public class NativeAuthPolicySql(IServerConfigService configService) : INativeAuthPolicy
{
    private String Connection => configService.Fetch().Database;

    public async Task<NativeAuthMethod?> Resolve(Guid userId)
    {
        return await NativeAuthPolicySelect.Execute(Connection, userId) switch
        {
            AuthProviders.PasswordEmailCode => NativeAuthMethod.PasswordEmailCode,
            AuthProviders.EmailCode => NativeAuthMethod.EmailCode,
            AuthProviders.StudentCode => NativeAuthMethod.StudentCode,
            _ => null,
        };
    }

    public async Task<ExternalAuthRoute?> ResolveExternal(Guid userId) =>
        await ExternalAuthPolicySelect.Execute(Connection, userId);
}