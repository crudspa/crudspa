using Crudspa.Framework.Auth.Shared.Contracts.Behavior;

namespace Crudspa.Framework.Auth.Server.Services;

public class AuthProviderRegistry(IEnumerable<IAuthProvider> providers)
{
    private readonly IReadOnlyDictionary<String, IAuthProvider> _providers =
        providers.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);

    public IEnumerable<IAuthProvider> Enabled => _providers.Values.Where(x => x.Enabled);

    public IAuthProvider? Find(String key)
    {
        return _providers.TryGetValue(key, out var provider) ? provider : null;
    }
}