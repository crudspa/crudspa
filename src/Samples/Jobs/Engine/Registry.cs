using Crudspa.Framework.Core.Server.Services;
using Crudspa.Framework.Core.Server.Wrappers;
using Crudspa.Framework.Jobs.Server.Services;

namespace Crudspa.Samples.Jobs.Engine;

public class Registry
{
    public static void RegisterServices(IServiceCollection services, ServerConfig config)
    {
        // Crudspa.Framework.Core.Server
        services.AddByTypeName<IBlobService>(config.BlobService);
        services.AddSingleton<ICryptographyService, CryptographyServiceCore>();
        services.AddSingleton<IGatewayService, GatewayServiceEventGrid>();
        services.AddSingleton<IServerConfigService, ServerConfigServiceCore>();
        services.AddSingleton<IServiceWrappers, ServiceWrappersCore>();
        services.AddSingleton<ISqlWrappers, SqlWrappersCore>();

        // Crudspa.Framework.Jobs.Shared
        services.AddSingleton<IFrameworkActionService, FrameworkActionServiceSql>();
        services.AddSingleton<IJobRunService, JobRunServiceSql>();
        services.AddSingleton<IJobScheduleService, JobScheduleServiceSql>();
        services.AddSingleton<IJobsConfigService, JobsConfigServiceUtilities>();
    }
}