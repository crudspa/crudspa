namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class SmsPreferencePayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class SmsPreferenceAdded : SmsPreferencePayload;

public class SmsPreferenceSaved : SmsPreferencePayload;

public class SmsPreferenceRemoved : SmsPreferencePayload;