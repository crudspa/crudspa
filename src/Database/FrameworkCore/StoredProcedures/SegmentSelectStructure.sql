create proc [FrameworkCore].[SegmentSelectStructure] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
    segment.Id
    ,segment.TypeId
    ,segment.ConfigJson
    ,type.Name as TypeName
    ,type.DisplayView as TypeDisplayView
    ,type.EditorView as TypeEditorView
from [Framework].[Segment-Active] segment
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[SegmentType-Active] type on segment.TypeId = type.Id
where segment.Id = @Id
    and organization.Id = @organizationId

select
    pane.Id
    ,pane.[Key]
    ,pane.Title
    ,pane.SegmentId
    ,pane.TypeId
    ,pane.PermissionId
    ,pane.ConfigJson
    ,pane.Ordinal
    ,type.Name as TypeName
from [Framework].[Pane-Active] pane
    inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[PaneType-Active] type on pane.TypeId = type.Id
where pane.SegmentId = @Id
    and organization.Id = @organizationId