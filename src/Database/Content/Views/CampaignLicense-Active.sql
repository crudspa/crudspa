create view [Content].[CampaignLicense-Active] as

select campaignLicense.Id as Id
    ,campaignLicense.CampaignId as CampaignId
    ,campaignLicense.LicenseId as LicenseId
from [Content].[CampaignLicense] campaignLicense
where 1=1
    and campaignLicense.IsDeleted = 0
    and campaignLicense.VersionOf = campaignLicense.Id