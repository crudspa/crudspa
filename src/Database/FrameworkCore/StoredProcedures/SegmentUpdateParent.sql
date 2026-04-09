create proc [FrameworkCore].[SegmentUpdateParent] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@ParentId uniqueidentifier
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

declare @existingPortalId uniqueidentifier
declare @existingParentId uniqueidentifier
declare @existingOrdinal int

select
     @existingPortalId = segment.PortalId
    ,@existingParentId = segment.ParentId
    ,@existingOrdinal = segment.Ordinal
from [Framework].[Segment-Active] segment
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where segment.Id = @Id
    and organization.Id = @organizationId

if @existingPortalId is null
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if (@ParentId is not null)
begin
    set @PortalId = (
        select top 1 segment.PortalId
        from [Framework].[Segment-Active] segment
            inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
            inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
        where segment.Id = @ParentId
            and organization.Id = @organizationId
    )
end

if (@ParentId is null and not exists (
    select 1
    from [Framework].[Portal-Active] portal
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where portal.Id = @PortalId
        and organization.Id = @organizationId
))
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if @PortalId is null
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

declare @newOrdinal int = (
    select isnull(max(Ordinal), -1) + 1
    from [Framework].[Segment-Active]
    where PortalId = @PortalId
        and (ParentId = @ParentId or (ParentId is null and @ParentId is null))
)

    update baseTable
    set
         Updated = @now
        ,UpdatedBy = @SessionId
        ,PortalId = @PortalId
        ,ParentId = @ParentId
        ,Ordinal = @newOrdinal
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

    ;with descendants as (
        select segment.Id
    from [Framework].[Segment-Active] segment
    where segment.ParentId = @Id

    union all

    select child.Id
    from [Framework].[Segment-Active] child
        inner join descendants ancestor on ancestor.Id = child.ParentId
)
    update baseTable
    set
         Updated = @now
        ,UpdatedBy = @SessionId
        ,PortalId = @PortalId
    from [Framework].[Segment] baseTable
        inner join descendants on descendants.Id = baseTable.Id
    where baseTable.PortalId <> @PortalId
    option (maxrecursion 32767)

    update baseTable
    set
         Updated = @now
        ,UpdatedBy = @SessionId
        ,Ordinal = baseTable.Ordinal - 1
    from [Framework].[Segment] baseTable
        inner join [Framework].[Segment-Active] segment on segment.Id = baseTable.Id
    where segment.PortalId = @existingPortalId
        and (segment.ParentId = @existingParentId or (segment.ParentId is null and @existingParentId is null))
        and segment.Ordinal > @existingOrdinal

commit transaction