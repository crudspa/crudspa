create proc [ContentMessaging].[StageInsert] (
     @SessionId uniqueidentifier
    ,@CampaignId uniqueidentifier
    ,@Name nvarchar(75)
    ,@Offset int
    ,@Anchor int
    ,@WeekendAdjustment int
    ,@SendTime time
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

declare @portalId uniqueidentifier = (
    select PortalId from [Content].[Campaign-Active] where Id = @CampaignId
)

if @portalId is null
    or not exists (select 1 from [ContentMessaging].[SessionOwnsPortal](@SessionId, @portalId))
    throw 51000, 'Stage access denied.', 1

begin transaction

declare @ordinal int = (select count(1) from [Content].[Stage-Active] where CampaignId = @CampaignId)

insert [Content].[Stage] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,CampaignId
    ,Name
    ,Offset
    ,Anchor
    ,WeekendAdjustment
    ,SendTime
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@CampaignId
    ,@Name
    ,@Offset
    ,@Anchor
    ,@WeekendAdjustment
    ,@SendTime
    ,@ordinal
)

commit transaction