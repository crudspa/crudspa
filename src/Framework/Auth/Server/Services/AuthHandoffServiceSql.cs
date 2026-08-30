using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Auth.Server.Sproxies;
using Crudspa.Framework.Core.Server.Contracts.Behavior;

namespace Crudspa.Framework.Auth.Server.Services;

public class AuthHandoffServiceSql(
    IServerConfigService configService,
    ICryptographyService cryptographyService)
{
    private String Connection => configService.Fetch().Database;

    public async Task<AuthHandoffRedemption?> Redeem(String code, Guid portalId, Guid? previousSessionId)
    {
        return await AuthHandoffRedeem.Execute(
            Connection,
            cryptographyService.ComputeHash(code),
            portalId,
            cryptographyService.GetRandomGuid(),
            previousSessionId);
    }
}