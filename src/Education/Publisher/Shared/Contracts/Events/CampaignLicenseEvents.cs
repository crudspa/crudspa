namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class CampaignLicensePayload
{
    public Guid? Id { get; set; }
    public Guid? LicenseId { get; set; }
}

public class CampaignLicenseAdded : CampaignLicensePayload;

public class CampaignLicenseSaved : CampaignLicensePayload;

public class CampaignLicenseRemoved : CampaignLicensePayload;