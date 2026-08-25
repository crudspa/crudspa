create proc [EducationPublisher].[CampaignLicenseSelectForLicense] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     campaignLicense.Id
    ,campaignLicense.LicenseId
    ,campaignLicense.CampaignId
    ,campaign.Name as CampaignName
from [Content].[CampaignLicense-Active] campaignLicense
    inner join [Content].[Campaign-Active] campaign on campaignLicense.CampaignId = campaign.Id
    inner join [Framework].[License-Active] license on campaignLicense.LicenseId = license.Id
    inner join [Framework].[Portal-Active] portal on campaign.PortalId = portal.Id
where campaignLicense.LicenseId = @LicenseId
    and portal.OwnerId = @organizationId
    and license.OwnerId = @organizationId