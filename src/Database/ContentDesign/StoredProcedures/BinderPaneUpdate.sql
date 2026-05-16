create proc [ContentDesign].[BinderPaneUpdate] (
     @SessionId uniqueidentifier
    ,@PaneId uniqueidentifier
    ,@BinderId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on

if not exists (
    select 1
    from [Framework].[Pane-Active] pane
        inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where pane.Id = @PaneId
        and organization.Id = @organizationId
)
begin
    ;throw 50000, 'Pane was not found.', 1
end

if @BinderId is not null and not exists (select 1 from [Content].[Binder-Active] where Id = @BinderId)
begin
    ;throw 50000, 'Pane BinderId does not reference an active Binder.', 1
end

declare @binderPaneId uniqueidentifier = (
    select top 1 Id
    from [Content].[BinderPane-Active]
    where PaneId = @PaneId
    order by Id
)

if @BinderId is null
begin
    update [Content].[BinderPane]
    set  Updated = @now
        ,UpdatedBy = @SessionId
        ,IsDeleted = 1
    where PaneId = @PaneId
        and Id = VersionOf
        and IsDeleted = 0
end
else
begin
    if @binderPaneId is null
    begin
        set @binderPaneId = newid()

        insert [Content].[BinderPane] (Id, VersionOf, Updated, UpdatedBy, PaneId, BinderId)
        values (@binderPaneId, @binderPaneId, @now, @SessionId, @PaneId, @BinderId)
    end
    else
    begin
        update [Content].[BinderPane]
        set  Updated = @now
            ,UpdatedBy = @SessionId
            ,BinderId = @BinderId
        where Id = @binderPaneId
            and BinderId <> @BinderId
    end

    update [Content].[BinderPane]
    set  Updated = @now
        ,UpdatedBy = @SessionId
        ,IsDeleted = 1
    where PaneId = @PaneId
        and Id = VersionOf
        and IsDeleted = 0
        and Id <> @binderPaneId
end