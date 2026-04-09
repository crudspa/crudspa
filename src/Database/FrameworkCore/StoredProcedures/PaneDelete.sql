create proc [FrameworkCore].[PaneDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @segmentId uniqueidentifier = (
    select top 1 SegmentId
    from [Framework].[Pane-Active] pane
        inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where pane.Id = @Id
        and organization.Id = @organizationId
)

declare @oldOrdinal int = (
    select top 1 pane.Ordinal
    from [Framework].[Pane-Active] pane
        inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where pane.Id = @Id
        and organization.Id = @organizationId
)

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Framework].[Pane] baseTable
    inner join [Framework].[Pane-Active] pane on pane.Id = baseTable.Id
    inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Framework].[Pane] baseTable
    inner join [Framework].[Pane-Active] pane on pane.Id = baseTable.Id
where pane.SegmentId = @segmentId
    and pane.Ordinal > @oldOrdinal

commit transaction