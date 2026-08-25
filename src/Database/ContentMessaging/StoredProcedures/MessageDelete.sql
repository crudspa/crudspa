create proc [ContentMessaging].[MessageDelete] (@SessionId uniqueidentifier,@Id uniqueidentifier) as
set nocount on
set xact_abort on
declare @now datetimeoffset=sysdatetimeoffset()
if not exists(select 1 from [Content].[Message-Active] message inner join [Content].[Stage-Active] stage on stage.Id=message.StageId inner join [Content].[Campaign-Active] campaign on campaign.Id=stage.CampaignId cross apply [ContentMessaging].[SessionOwnsPortal](@SessionId,campaign.PortalId) where message.Id=@Id and message.ActivationId is null) throw 51000,'Message access denied.',1
begin transaction
update [Content].[Message] set Id=@Id,Updated=@now,UpdatedBy=@SessionId,IsDeleted=1 where Id=@Id
commit transaction