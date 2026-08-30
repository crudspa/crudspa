using Crudspa.Framework.Auth.Server.Extensions;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Server.Services;
using Crudspa.Integrations.Clever.Server.Extensions;

namespace Crudspa.Samples.Auth.Server;

public static class Registry
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICryptographyService, CryptographyServiceCore>();
        services.AddSingleton<IServerConfigService, ServerConfigServiceCore>();
        services.AddAuth(configuration);
        services.AddCleverAuth(configuration);
    }
}