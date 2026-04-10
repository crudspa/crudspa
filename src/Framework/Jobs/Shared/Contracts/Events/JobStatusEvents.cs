namespace Crudspa.Framework.Jobs.Shared.Contracts.Events;

public class JobStatusChanged : JobPayload
{
    public DateTimeOffset? Started { get; set; }
    public DateTimeOffset? Ended { get; set; }
    public Guid? StatusId { get; set; }
    public Guid? DeviceId { get; set; }
}