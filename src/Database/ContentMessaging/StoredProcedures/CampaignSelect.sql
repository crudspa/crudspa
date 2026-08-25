create proc [ContentMessaging].[CampaignSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

declare @organizationId uniqueidentifier = (
    select portal.OwnerId
    from [Content].[Campaign-Active] campaign
        inner join [Framework].[Portal-Active] portal on portal.Id = campaign.PortalId
        cross apply [ContentMessaging].[SessionOwnsPortal](@SessionId, campaign.PortalId)
    where campaign.Id = @Id
)

select
     campaign.Id
    ,campaign.PortalId
    ,campaign.Name
    ,campaign.Description
from [Content].[Campaign-Active] campaign
    inner join [Framework].[Portal-Active] portal on campaign.PortalId = portal.Id
    cross apply [ContentMessaging].[SessionOwnsPortal](@SessionId, campaign.PortalId)
where campaign.Id = @Id

select distinct
     @Id as CampaignId
    ,license.Id as LicenseId
    ,license.Name as LicenseName
    ,convert(bit, iif(campaignLicense.Id is null, 0, 1)) as Selected
from [Framework].[License-Active] license
    left join [Content].[CampaignLicense-Active] campaignLicense on campaignLicense.LicenseId = license.Id
        and campaignLicense.CampaignId = @Id
where license.OwnerId = @organizationId
order by license.Name