namespace Crudspa.Framework.Jobs.Shared.Contracts.Data;

public class JobsConfig
{
    public Guid DeviceId { get; set; } = Guid.Empty;
    public String OpenAiApiKey { get; set; } = null!;
    public Int32 PollingInterval { get; set; } = 0;
    public Int32 SchedulingInterval { get; set; } = 0;
}