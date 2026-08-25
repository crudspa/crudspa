create proc [ContentMessaging].[StageDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[Stage-Active] stage
        inner join [Content].[Campaign-Active] campaign on stage.CampaignId = campaign.Id
        cross apply [ContentMessaging].[SessionOwnsPortal](@SessionId, campaign.PortalId)
    where stage.Id = @Id
)
    throw 51000, 'Stage access denied.', 1

begin transaction

declare @campaignId uniqueidentifier = (
    select top 1 CampaignId
    from [Content].[Stage-Active] stage
    where stage.Id = @Id
)

declare @oldOrdinal int = (
    select top 1 Ordinal
    from [Content].[Stage-Active] stage
    where stage.Id = @Id
)

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Stage] baseTable
    inner join [Content].[Stage-Active] stage on stage.Id = baseTable.Id
where baseTable.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Content].[Stage] baseTable
    inner join [Content].[Stage-Active] stage on stage.Id = baseTable.Id
where stage.CampaignId = @campaignId
    and stage.Ordinal > @oldOrdinal

commit transaction