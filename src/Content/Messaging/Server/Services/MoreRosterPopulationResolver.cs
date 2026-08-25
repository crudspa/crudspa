namespace Crudspa.Content.Messaging.Server.Services;

public class MoreRosterPopulationResolver(IServerConfigService configService) : IPopulationResolver
{
    public String Key => "more-roster";
    public IList<PopulationToken> Tokens => MoreRosterSelect.CreateTokens();

    public async Task<PopulationResult> Resolve(Guid? sessionId, Population population, Guid organizationId, Guid? activationScopeId) =>
        await MoreRosterSelect.Execute(
            configService.Fetch().Database,
            sessionId,
            population.PortalId,
            organizationId,
            population.Key,
            activationScopeId);
}