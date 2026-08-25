namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface IPopulationResolver
{
    String Key { get; }
    IList<PopulationToken> Tokens { get; }
    Task<PopulationResult> Resolve(Guid? sessionId, Population population, Guid organizationId, Guid? activationScopeId);
}