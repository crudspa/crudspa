namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class EmailTemplatePayload
{
    public Guid? Id { get; set; }
    public Guid? MembershipId { get; set; }
}

public class EmailTemplateAdded : EmailTemplatePayload;

public class EmailTemplateSaved : EmailTemplatePayload;

public class EmailTemplateRemoved : EmailTemplatePayload;