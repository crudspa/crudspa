create proc [FrameworkCore].[PaneUpdateSegment] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@SegmentId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()

declare @existingSegmentId uniqueidentifier
declare @existingPortalId uniqueidentifier
declare @existingOrdinal int
declare @existingKey nvarchar(75)

select
     @existingSegmentId = pane.SegmentId
    ,@existingPortalId = segment.PortalId
    ,@existingOrdinal = pane.Ordinal
    ,@existingKey = pane.[Key]
from [Framework].[Pane-Active] pane
    inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where pane.Id = @Id
    and organization.Id = @organizationId

if @existingSegmentId is null
begin
    raiserror('Tenancy check failed', 16, 1)
    return
end

declare @destinationPortalId uniqueidentifier = (
    select top 1 segment.PortalId
    from [Framework].[Segment-Active] segment
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where segment.Id = @SegmentId
        and organization.Id = @organizationId
)

if @destinationPortalId is null
begin
    raiserror('Tenancy check failed', 16, 1)
    return
end

declare @newOrdinal int = (
    select isnull(max(pane.Ordinal), -1) + 1
    from [Framework].[Pane-Active] pane
    where pane.SegmentId = @SegmentId
)

declare @newKey nvarchar(75) = @existingKey

if @newKey is not null
begin
    if exists (
        select 1
        from [Framework].[Pane-Active] pane
        where pane.SegmentId = @SegmentId
            and pane.Id != @Id
            and pane.[Key] = @newKey
    )
    begin
        declare @i int = 2

        while exists (
            select 1
            from [Framework].[Pane-Active] pane
            where pane.SegmentId = @SegmentId
                and pane.Id != @Id
                and pane.[Key] = @existingKey + cast(@i as nvarchar(10))
        )
            set @i = @i + 1

        set @newKey = @existingKey + cast(@i as nvarchar(10))
    end
end

declare @tabbedSegmentTypeId uniqueidentifier = 'e86d3ee2-22df-4cb1-bb66-ea417d34edeb'

begin transaction

    update baseTable
    set  Updated = @now
        ,UpdatedBy = @SessionId
        ,TypeId = @tabbedSegmentTypeId
    from [Framework].[Segment] baseTable
        inner join [Framework].[Segment-Active] segment on segment.Id = baseTable.Id
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where baseTable.Id = @SegmentId
        and organization.Id = @organizationId
        and baseTable.TypeId != @tabbedSegmentTypeId

    update baseTable
    set  Updated = @now
        ,UpdatedBy = @SessionId
        ,SegmentId = @SegmentId
        ,Ordinal = @newOrdinal
        ,[Key] = @newKey
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

    update [Framework].[Pane]
    set  Updated = @now
        ,UpdatedBy = @SessionId
        ,Ordinal = Ordinal - 1
    where SegmentId = @existingSegmentId
        and Ordinal > @existingOrdinal

    if not exists (
        select 1
        from [Framework].[Pane-Active] pane
        where pane.SegmentId = @existingSegmentId
    )
    begin
        declare @paneTypeId uniqueidentifier = (
            select top 1 paneType.Id
            from [Framework].[PaneType-Active] paneType
                inner join [Framework].[PortalPaneType-Active] portalPaneType on portalPaneType.TypeId = paneType.Id
            where portalPaneType.PortalId = @existingPortalId
            order by paneType.Name
        )

        declare @blankPaneId uniqueidentifier = newid()

        insert [Framework].[Pane] (
             Id
            ,VersionOf
            ,Updated
            ,UpdatedBy
            ,SegmentId
            ,[Key]
            ,Title
            ,TypeId
            ,ConfigJson
            ,Ordinal
        )
        values (
             @blankPaneId
            ,@blankPaneId
            ,@now
            ,@SessionId
            ,@existingSegmentId
            ,'details'
            ,'Details'
            ,@paneTypeId
            ,null
            ,0
        )
    end

commit transaction