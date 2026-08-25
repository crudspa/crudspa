using Crudspa.Content.Display.Server.Contracts.Behavior;

namespace Crudspa.Education.Common.Server.Services;

public class SessionLicenseResolverEducationSql(IServerConfigService configService) : ISessionLicenseResolver
{
    private String Connection => configService.Fetch().Database;

    public async Task<IReadOnlyCollection<Guid?>> Fetch(Guid? sessionId) =>
        (await SessionLicenseSelect.Execute(Connection, sessionId)).ToList();
}