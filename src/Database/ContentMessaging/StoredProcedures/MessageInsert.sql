create proc [ContentMessaging].[MessageInsert] (@SessionId uniqueidentifier,@StageId uniqueidentifier,@Name nvarchar(75),@PopulationId uniqueidentifier,@MessageTypeId uniqueidentifier,@EmailTemplateId uniqueidentifier,@SmsTemplateId uniqueidentifier,@Id uniqueidentifier output) as
set nocount on
set xact_abort on
set @Id=newid()
declare @now datetimeoffset=sysdatetimeoffset()
declare @portalId uniqueidentifier=(select campaign.PortalId from [Content].[Stage-Active] stage inner join [Content].[Campaign-Active] campaign on campaign.Id=stage.CampaignId where stage.Id=@StageId)
if @portalId is null or not exists(select 1 from [ContentMessaging].[SessionOwnsPortal](@SessionId,@portalId)) throw 51000,'Message access denied.',1
if not exists(select 1 from [Content].[Population-Active] where Id=@PopulationId and PortalId=@portalId)
 or (@EmailTemplateId is not null and not exists(select 1 from [Content].[EmailTemplate-Active] where Id=@EmailTemplateId and PortalId=@portalId and MembershipId is null and OrganizationId is null))
 or (@SmsTemplateId is not null and not exists(select 1 from [Content].[SmsTemplate-Active] where Id=@SmsTemplateId and PortalId=@portalId and OrganizationId is null)) throw 51000,'Message configuration is outside the Campaign Portal.',1
begin transaction
declare @ordinal int=(select count(*) from [Content].[Message-Active] where StageId=@StageId and ActivationId is null)
insert [Content].[Message](Id,VersionOf,Updated,UpdatedBy,StageId,Name,PopulationId,MessageTypeId,EmailTemplateId,SmsTemplateId,Ordinal)
values(@Id,@Id,@now,@SessionId,@StageId,@Name,@PopulationId,@MessageTypeId,@EmailTemplateId,@SmsTemplateId,@ordinal)
commit transaction