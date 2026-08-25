namespace Crudspa.Content.Messaging.Server.Services;

public class PopulationResolverRegistry
{
    private readonly IReadOnlyDictionary<String, IPopulationResolver> _resolvers;

    public PopulationResolverRegistry(IEnumerable<IPopulationResolver> resolvers)
    {
        var list = resolvers.ToList();
        var duplicate = list.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase).FirstOrDefault(x => x.Count() > 1);

        if (duplicate is not null)
            throw new InvalidOperationException($"Population resolver key '{duplicate.Key}' is registered more than once.");

        _resolvers = list.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
    }

    public IPopulationResolver Fetch(String key)
    {
        if (_resolvers.TryGetValue(key, out var resolver))
            return resolver;

        throw new InvalidOperationException($"Population resolver key '{key}' is not registered.");
    }
}

public class ActivationTargetProviderRegistry
{
    private readonly IReadOnlyDictionary<Guid, IActivationTargetProvider> _providers;

    public ActivationTargetProviderRegistry(IEnumerable<IActivationTargetProvider> providers)
    {
        var list = providers.ToList();
        var duplicate = list.GroupBy(x => x.PortalId).FirstOrDefault(x => x.Count() > 1);

        if (duplicate is not null)
            throw new InvalidOperationException($"Activation target provider '{duplicate.Key}' is registered more than once.");

        _providers = list.ToDictionary(x => x.PortalId);
    }

    public IActivationTargetProvider Fetch(Guid portalId)
    {
        if (_providers.TryGetValue(portalId, out var provider))
            return provider;

        throw new InvalidOperationException($"No Activation target provider is registered for Portal '{portalId}'.");
    }
}