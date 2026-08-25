namespace Crudspa.Content.Messaging.Server.Services;

/// <summary>
/// Resolves a Population from its unscoped template Membership.
/// </summary>
public class StaticMembershipPopulationResolver(IServerConfigService configService) : IPopulationResolver
{
    public String Key => "static-membership";
    public IList<PopulationToken> Tokens => StaticMembershipSelect.CreateTokens();

    public async Task<PopulationResult> Resolve(Guid? sessionId, Population population, Guid organizationId, Guid? activationScopeId) =>
        await StaticMembershipSelect.Execute(
            configService.Fetch().Database,
            sessionId,
            population.PortalId,
            population.Id,
            organizationId);
}