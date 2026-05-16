namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class SmsMessageMediaPayload
{
    public Guid? Id { get; set; }
    public Guid? SmsMessageId { get; set; }
}

public class SmsMessageMediasReordered : SmsMessageMediaPayload;