create proc [ContentMessaging].[MessageSelectForStage] (@SessionId uniqueidentifier, @StageId uniqueidentifier) as
set nocount on
select message.Id,message.StageId,message.Name,message.PopulationId,message.MessageTypeId,message.EmailTemplateId,message.SmsTemplateId,message.Ordinal
from [Content].[Message-Active] message
inner join [Content].[Stage-Active] stage on stage.Id=message.StageId
inner join [Content].[Campaign-Active] campaign on campaign.Id=stage.CampaignId
cross apply [ContentMessaging].[SessionOwnsPortal](@SessionId,campaign.PortalId)
where message.StageId=@StageId and message.ActivationId is null
order by message.Ordinal