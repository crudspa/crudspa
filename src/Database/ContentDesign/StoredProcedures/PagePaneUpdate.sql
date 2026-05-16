create proc [ContentDesign].[PagePaneUpdate] (
     @SessionId uniqueidentifier
    ,@PaneId uniqueidentifier
    ,@PageId uniqueidentifier
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

if @PageId is not null and not exists (select 1 from [Content].[Page-Active] where Id = @PageId)
begin
    ;throw 50000, 'Pane PageId does not reference an active Page.', 1
end

declare @pagePaneId uniqueidentifier = (
    select top 1 Id
    from [Content].[PagePane-Active]
    where PaneId = @PaneId
    order by Id
)

if @PageId is null
begin
    update [Content].[PagePane]
    set  Updated = @now
        ,UpdatedBy = @SessionId
        ,IsDeleted = 1
    where PaneId = @PaneId
        and Id = VersionOf
        and IsDeleted = 0
end
else
begin
    if @pagePaneId is null
    begin
        set @pagePaneId = newid()

        insert [Content].[PagePane] (Id, VersionOf, Updated, UpdatedBy, PaneId, PageId)
        values (@pagePaneId, @pagePaneId, @now, @SessionId, @PaneId, @PageId)
    end
    else
    begin
        update [Content].[PagePane]
        set  Updated = @now
            ,UpdatedBy = @SessionId
            ,PageId = @PageId
        where Id = @pagePaneId
            and PageId <> @PageId
    end

    update [Content].[PagePane]
    set  Updated = @now
        ,UpdatedBy = @SessionId
        ,IsDeleted = 1
    where PaneId = @PaneId
        and Id = VersionOf
        and IsDeleted = 0
        and Id <> @pagePaneId
end