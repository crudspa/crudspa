create trigger [Content].[CampaignLicenseTrigger] on [Content].[CampaignLicense]
    for update
as

insert [Content].[CampaignLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,CampaignId
    ,LicenseId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.CampaignId
    ,deleted.LicenseId
from deleted