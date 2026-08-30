using Crudspa.Framework.Auth.Shared.Contracts.Behavior;
using Crudspa.Integrations.Clever.Server.Contracts.Ids;

namespace Crudspa.Integrations.Clever.Server.Services;

public class CleverAuthProvider(Boolean enabled) : IAuthProvider
{
    public String Key => CleverAuthSchemes.Provider;
    public String ChallengeScheme => CleverAuthSchemes.Challenge;
    public String SessionScheme => CleverAuthSchemes.Session;
    public Boolean Enabled => enabled;
}