create proc [ContentMessaging].[MessageUpdate] (@SessionId uniqueidentifier,@Id uniqueidentifier,@StageId uniqueidentifier,@Name nvarchar(75),@PopulationId uniqueidentifier,@MessageTypeId uniqueidentifier,@EmailTemplateId uniqueidentifier,@SmsTemplateId uniqueidentifier) as
set nocount on
set xact_abort on
declare @now datetimeoffset=sysdatetimeoffset()
declare @portalId uniqueidentifier=(select campaign.PortalId from [Content].[Message-Active] message inner join [Content].[Stage-Active] stage on stage.Id=message.StageId inner join [Content].[Campaign-Active] campaign on campaign.Id=stage.CampaignId where message.Id=@Id and message.ActivationId is null)
if @portalId is null or not exists(select 1 from [ContentMessaging].[SessionOwnsPortal](@SessionId,@portalId)) throw 51000,'Message access denied.',1
if not exists(select 1 from [Content].[Population-Active] where Id=@PopulationId and PortalId=@portalId)
 or (@EmailTemplateId is not null and not exists(select 1 from [Content].[EmailTemplate-Active] where Id=@EmailTemplateId and PortalId=@portalId and MembershipId is null and OrganizationId is null))
 or (@SmsTemplateId is not null and not exists(select 1 from [Content].[SmsTemplate-Active] where Id=@SmsTemplateId and PortalId=@portalId and OrganizationId is null)) throw 51000,'Message configuration is outside the Campaign Portal.',1
begin transaction
update baseTable set Id=@Id,Updated=@now,UpdatedBy=@SessionId,Name=@Name,PopulationId=@PopulationId,MessageTypeId=@MessageTypeId,EmailTemplateId=@EmailTemplateId,SmsTemplateId=@SmsTemplateId
from [Content].[Message] baseTable inner join [Content].[Message-Active] message on message.Id=baseTable.Id where baseTable.Id=@Id
commit transaction