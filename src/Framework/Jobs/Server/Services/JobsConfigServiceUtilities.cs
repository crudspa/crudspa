namespace Crudspa.Framework.Jobs.Server.Services;

public class JobsConfigServiceUtilities(IConfiguration configuration) : IJobsConfigService
{
    private JobsConfig? _jobsConfig;

    public JobsConfig Fetch()
    {
        return _jobsConfig ??= new()
        {
            DeviceId = configuration.ReadGuid("Crudspa.Framework.Jobs.Utilities.DeviceId"),
            OpenAiApiKey = configuration.ReadString("Crudspa.Framework.Jobs.Utilities.OpenAiApiKey"),
            PollingInterval = configuration.ReadInt("Crudspa.Framework.Jobs.Utilities.PollingInterval"),
            SchedulingInterval = configuration.ReadInt("Crudspa.Framework.Jobs.Utilities.SchedulingInterval"),
        };
    }

    public void Invalidate()
    {
        _jobsConfig = null;
    }
}