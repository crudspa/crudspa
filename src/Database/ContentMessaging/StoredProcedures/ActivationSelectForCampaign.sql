create proc [ContentMessaging].[ActivationSelectForCampaign] (
     @SessionId uniqueidentifier
    ,@CampaignId uniqueidentifier
) as

set nocount on

select
     activation.Id
    ,activation.OrganizationId
    ,organization.Name as OrganizationName
    ,activation.CampaignId
    ,campaign.Name as CampaignName
    ,activation.BatchId
    ,activation.Start
    ,activation.Activated
    ,activation.ActivatedBy
from [Content].[Activation-Active] activation
    inner join [Content].[Campaign-Active] campaign on activation.CampaignId = campaign.Id
    inner join [Framework].[Organization-Active] organization on activation.OrganizationId = organization.Id
    cross apply [ContentMessaging].[SessionOwnsPortal](@SessionId, campaign.PortalId)
where activation.CampaignId = @CampaignId