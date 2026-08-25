namespace Crudspa.Content.Messaging.Server.Services;

public class MoreEngagementPopulationResolver(IServerConfigService configService) : IPopulationResolver
{
    public String Key => "more-engagement";
    public IList<PopulationToken> Tokens => MoreEngagementSelect.CreateTokens();

    public async Task<PopulationResult> Resolve(Guid? sessionId, Population population, Guid organizationId, Guid? activationScopeId) =>
        await MoreEngagementSelect.Execute(
            configService.Fetch().Database,
            sessionId,
            population.PortalId,
            organizationId,
            population.Key,
            activationScopeId);
}