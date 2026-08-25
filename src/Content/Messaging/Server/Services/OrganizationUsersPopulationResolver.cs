namespace Crudspa.Content.Messaging.Server.Services;

/// <summary>
/// Non-confidential sample resolver for active users in the target Organization and Portal.
/// </summary>
public class OrganizationUsersPopulationResolver(IServerConfigService configService) : IPopulationResolver
{
    public String Key => "organization-users";
    public IList<PopulationToken> Tokens => OrganizationUsersSelect.CreateTokens();

    public async Task<PopulationResult> Resolve(Guid? sessionId, Population population, Guid organizationId, Guid? activationScopeId) =>
        await OrganizationUsersSelect.Execute(
            configService.Fetch().Database,
            sessionId,
            population.PortalId,
            organizationId);
}