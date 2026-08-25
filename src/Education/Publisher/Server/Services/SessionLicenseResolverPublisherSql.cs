using Crudspa.Content.Display.Server.Contracts.Behavior;

namespace Crudspa.Education.Publisher.Server.Services;

public class SessionLicenseResolverPublisherSql(IServerConfigService configService) : ISessionLicenseResolver
{
    private String Connection => configService.Fetch().Database;

    public async Task<IReadOnlyCollection<Guid?>> Fetch(Guid? sessionId) =>
        (await SessionLicenseSelectForPublisher.Execute(Connection, sessionId)).ToList();
}