create proc [FrameworkCore].[SegmentDelete] (
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

declare @portalId uniqueidentifier = (
    select top 1 PortalId
    from [Framework].[Segment-Active] segment
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where segment.Id = @Id
    and organization.Id = @organizationId
)

declare @parentId uniqueidentifier = (
    select top 1 ParentId
    from [Framework].[Segment-Active] segment
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where segment.Id = @Id
    and organization.Id = @organizationId
)

declare @oldOrdinal int = (
    select top 1 Ordinal
    from [Framework].[Segment-Active] segment
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where segment.Id = @Id
    and organization.Id = @organizationId
)

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Framework].[Segment] baseTable
    inner join [Framework].[Segment-Active] segment on segment.Id = baseTable.Id
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
from [Framework].[Segment] baseTable
    inner join [Framework].[Segment-Active] segment on segment.Id = baseTable.Id
where segment.PortalId = @portalId
    and (
        (@parentId is null and segment.ParentId is null)
        or (@parentId is not null and segment.ParentId = @parentId)
    )
    and segment.Ordinal > @oldOrdinal

commit transaction