using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Auth.Server.Sproxies;
using Crudspa.Framework.Core.Server.Contracts.Behavior;

namespace Crudspa.Framework.Auth.Server.Services;

public class AuthStartPolicyServiceSql(IServerConfigService configService)
{
    private String Connection => configService.Fetch().Database;

    public async Task<AuthStartPolicy?> Resolve(String? provider, String audience, String tenant)
    {
        var policies = await AuthStartPolicySelect.Execute(Connection, provider, audience, tenant);
        return policies.Count == 1 ? policies[0] : null;
    }
}