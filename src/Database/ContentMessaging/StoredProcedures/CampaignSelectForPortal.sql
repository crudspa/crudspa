create proc [ContentMessaging].[CampaignSelectForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
) as

set nocount on

select
     campaign.Id
    ,campaign.PortalId
    ,campaign.Name
    ,campaign.Description
from [Content].[Campaign-Active] campaign
    inner join [Framework].[Portal-Active] portal on campaign.PortalId = portal.Id
    cross apply [ContentMessaging].[SessionOwnsPortal](@SessionId, campaign.PortalId)
where campaign.PortalId = @PortalId