namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class SmsTemplatePayload
{
    public Guid? Id { get; set; }
    public Guid? MembershipId { get; set; }
}

public class SmsTemplateAdded : SmsTemplatePayload;

public class SmsTemplateSaved : SmsTemplatePayload;

public class SmsTemplateRemoved : SmsTemplatePayload;