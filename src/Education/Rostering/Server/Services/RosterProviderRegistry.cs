using Crudspa.Education.Rostering.Shared.Contracts.Behavior;

namespace Crudspa.Education.Rostering.Server.Services;

public class RosterProviderRegistry(IEnumerable<IRosterProvider> providers)
{
    private readonly IReadOnlyDictionary<String, IRosterProvider> _providers =
        providers.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);

    public IRosterProvider? Find(String key) =>
        _providers.GetValueOrDefault(key);
}