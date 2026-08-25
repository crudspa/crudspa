namespace Crudspa.Content.Messaging.Shared.Contracts.Events;

public class SmsEventPayload
{
    public Guid? Id { get; set; }
    public Guid? SmsMessageId { get; set; }
}

public class SmsEventAdded : SmsEventPayload;

public class SmsEventSaved : SmsEventPayload;

public class SmsEventRemoved : SmsEventPayload;