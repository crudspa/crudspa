namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class SmsPayload
{
    public Guid? Id { get; set; }
    public Guid? MembershipId { get; set; }
}

public class SmsAdded : SmsPayload;

public class SmsSaved : SmsPayload;

public class SmsRemoved : SmsPayload;