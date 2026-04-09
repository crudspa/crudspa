create proc [FrameworkCore].[SegmentUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
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

if (
    select count(1)
    from @Orderables
) != (
    select count(1)
    from @Orderables orderable
        inner join [Framework].[Segment-Active] segment on segment.Id = orderable.Id
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if exists (select 1 from @Orderables) and 1 != (
    select count(1)
    from (
        select segment.PortalId, segment.ParentId
        from @Orderables orderable
            inner join [Framework].[Segment-Active] segment on segment.Id = orderable.Id
        group by segment.PortalId, segment.ParentId
    ) siblingGroups
)
begin
    rollback transaction
    raiserror('Segments must be siblings', 16, 1)
    return
end

update baseTable
set
     baseTable.Ordinal = orderable.Ordinal
    ,baseTable.Updated = @now
    ,baseTable.UpdatedBy = @SessionId
from [Framework].[Segment] baseTable
    inner join @Orderables orderable on orderable.Id = baseTable.Id
    inner join [Framework].[Segment-Active] segment on segment.Id = baseTable.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where organization.Id = @organizationId
    and baseTable.Ordinal != orderable.Ordinal

commit transaction