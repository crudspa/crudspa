using Crudspa.Framework.Core.Server.Extensions;

namespace Crudspa.Framework.Auth.Server.Contracts.Data;

public class SessionAuthConfig(IConfiguration configuration)
{
    private const String Prefix = "Crudspa.Framework.Auth.Server";

    public String CookieName { get; } = configuration.ReadString($"{Prefix}.CookieName");
    public String AntiforgeryCookieName => $"{CookieName}.Antiforgery";

    public void Validate()
    {
        if (!CookieName.StartsWith("__Host-", StringComparison.Ordinal))
            throw new InvalidOperationException("The portal authentication cookie must use the __Host- prefix.");
    }
}