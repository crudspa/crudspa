create proc [ContentMessaging].[StageUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(75)
    ,@Offset int
    ,@Anchor int
    ,@WeekendAdjustment int
    ,@SendTime time
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

declare @portalId uniqueidentifier = (
    select campaign.PortalId
    from [Content].[Stage-Active] stage
        inner join [Content].[Campaign-Active] campaign on stage.CampaignId = campaign.Id
    where stage.Id = @Id
)

if @portalId is null
    or not exists (select 1 from [ContentMessaging].[SessionOwnsPortal](@SessionId, @portalId))
    throw 51000, 'Stage access denied.', 1

begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Name = @Name
    ,Offset = @Offset
    ,Anchor = @Anchor
    ,WeekendAdjustment = @WeekendAdjustment
    ,SendTime = @SendTime
from [Content].[Stage] baseTable
    inner join [Content].[Stage-Active] stage on stage.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction