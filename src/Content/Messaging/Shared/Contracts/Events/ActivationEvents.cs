namespace Crudspa.Content.Messaging.Shared.Contracts.Events;

public class ActivationPayload
{
    public Guid? Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? CampaignId { get; set; }
}

public class ActivationAdded : ActivationPayload;

public class ActivationSaved : ActivationPayload;

public class ActivationRemoved : ActivationPayload;