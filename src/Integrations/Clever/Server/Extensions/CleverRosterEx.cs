using Crudspa.Education.Rostering.Shared.Contracts.Behavior;
using Crudspa.Integrations.Clever.Server.Contracts.Behavior;
using Crudspa.Integrations.Clever.Server.Contracts.Data;
using Crudspa.Integrations.Clever.Server.Services;

namespace Crudspa.Integrations.Clever.Server.Extensions;

public static class CleverRosterEx
{
    public static IServiceCollection AddCleverRoster(this IServiceCollection services, IConfiguration configuration)
    {
        var config = new CleverConfig(configuration);

        if (!config.Configured)
            return services;

        services.AddSingleton(config);
        services.AddHttpClient<CleverClient>(client => client.Timeout = TimeSpan.FromMinutes(2));
        services.AddSingleton<ICleverTokenSource, CleverTokenSource>();
        services.AddSingleton<IRosterProvider, CleverRosterProvider>();
        return services;
    }
}