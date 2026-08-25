create proc [ContentMessaging].[EmailStageSelectForActivation] (
     @SessionId uniqueidentifier
    ,@ActivationId uniqueidentifier
) as

set nocount on

select distinct
     stage.Id
    ,stage.Name
from [Content].[Activation-Active] activation
    inner join [Content].[Campaign-Active] campaign on campaign.Id = activation.CampaignId
    inner join [Content].[Stage-Active] stage on stage.CampaignId = campaign.Id
    cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, campaign.PortalId, activation.OrganizationId)
where activation.Id = @ActivationId
order by stage.Name, stage.Id