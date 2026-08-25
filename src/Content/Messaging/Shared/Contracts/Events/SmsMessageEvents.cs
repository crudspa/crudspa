namespace Crudspa.Content.Messaging.Shared.Contracts.Events;

public class SmsMessagePayload
{
    public Guid? Id { get; set; }
    public Guid? MembershipId { get; set; }
    public Guid? ContactPhoneId { get; set; }
    public Guid? ContactId { get; set; }
}

public class SmsMessageAdded : SmsMessagePayload;

public class SmsMessageSaved : SmsMessagePayload;

public class SmsMessageRemoved : SmsMessagePayload;