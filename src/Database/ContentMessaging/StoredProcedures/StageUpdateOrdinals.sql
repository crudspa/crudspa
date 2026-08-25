create proc [ContentMessaging].[StageUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if exists (
    select 1
    from @Orderables orderable
        inner join [Content].[Stage-Active] stage on orderable.Id = stage.Id
        inner join [Content].[Campaign-Active] campaign on stage.CampaignId = campaign.Id
    where not exists (select 1 from [ContentMessaging].[SessionOwnsPortal](@SessionId, campaign.PortalId))
)
    or exists (select 1 from @Orderables orderable where not exists (select 1 from [Content].[Stage-Active] stage where stage.Id = orderable.Id))
    throw 51000, 'Stage access denied.', 1

begin transaction

update stage
set
     stage.Ordinal = orderable.Ordinal
    ,stage.Updated = @now
    ,stage.UpdatedBy = @SessionId
from [Content].[Stage] stage
    inner join @Orderables orderable on orderable.Id = stage.Id
where stage.Ordinal != orderable.Ordinal

commit transaction