create proc [ContentDisplay].[PagePaneSelectForPane] (
     @SessionId uniqueidentifier
    ,@PaneId uniqueidentifier
) as

set nocount on

declare @contentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'
declare @portalId uniqueidentifier = null
declare @sessionUserId uniqueidentifier = null

select
     @portalId = session.PortalId
    ,@sessionUserId = session.UserId
from [Framework].[Session-Active] session
where session.Id = @SessionId
    and session.Ended is null

create table #sessionPermissions (
    PermissionId uniqueidentifier not null primary key
)

insert into #sessionPermissions (PermissionId)
select distinct rolePermission.PermissionId
from [Framework].[UserRole-Active] userRole
    inner join [Framework].[RolePermission-Active] rolePermission on rolePermission.RoleId = userRole.RoleId
    inner join [Framework].[PortalPermission-Active] portalPermission on portalPermission.PermissionId = rolePermission.PermissionId
where userRole.UserId = @sessionUserId
    and portalPermission.PortalId = @portalId

select top 1
     pagePane.Id
    ,pagePane.PaneId
    ,pagePane.PageId
from [Content].[PagePane-Active] pagePane
    inner join [Framework].[Pane-Active] pane on pagePane.PaneId = pane.Id
    inner join [Framework].[PaneType-Active] paneType on pane.TypeId = paneType.Id
    inner join [Framework].[PortalPaneType-Active] portalPaneType on portalPaneType.TypeId = paneType.Id
    inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
where pagePane.PaneId = @PaneId
    and segment.PortalId = @portalId
    and portalPaneType.PortalId = @portalId
    and segment.StatusId = @contentStatusComplete
    and (
        segment.PermissionId is null
        or exists (
            select 1
            from #sessionPermissions permission
            where permission.PermissionId = segment.PermissionId
        )
    )
    and (
        pane.PermissionId is null
        or exists (
            select 1
            from #sessionPermissions permission
            where permission.PermissionId = pane.PermissionId
        )
    )
order by pagePane.Id