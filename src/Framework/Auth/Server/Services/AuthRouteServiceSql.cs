using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Auth.Server.Sproxies;
using Crudspa.Framework.Core.Server.Contracts.Behavior;

namespace Crudspa.Framework.Auth.Server.Services;

public class AuthRouteServiceSql(IServerConfigService configService)
{
    private String Connection => configService.Fetch().Database;

    public async Task<IList<AuthRoute>> Fetch(String audience) =>
        await AuthRouteSelect.Execute(Connection, audience);

    public async Task<AuthRoute?> Find(String audience, String key)
    {
        var routes = await AuthRouteSelect.Execute(Connection, audience, key);
        return routes.Count == 1 ? routes[0] : null;
    }
}