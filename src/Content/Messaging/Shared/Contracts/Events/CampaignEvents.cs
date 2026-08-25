namespace Crudspa.Content.Messaging.Shared.Contracts.Events;

public class CampaignPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class CampaignAdded : CampaignPayload;

public class CampaignSaved : CampaignPayload;

public class CampaignRemoved : CampaignPayload;