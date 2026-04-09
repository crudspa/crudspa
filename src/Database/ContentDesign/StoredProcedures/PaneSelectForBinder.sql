create proc [ContentDesign].[PaneSelectForBinder] (
     @SessionId uniqueidentifier
    ,@BinderId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select top 1
     pane.Id
    ,pane.SegmentId
    ,pane.Title
    ,pane.[Key]
    ,pane.TypeId
    ,type.Name as TypeName
    ,type.EditorView as TypeEditorView
    ,pane.PermissionId
    ,permission.Name as PermissionName
    ,pane.ConfigJson
    ,pane.Ordinal
from [Framework].[Pane-Active] pane
    left join [Framework].[Permission-Active] permission on pane.PermissionId = permission.Id
    inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[PaneType-Active] type on pane.TypeId = type.Id
where try_convert(uniqueidentifier, json_value(pane.ConfigJson, '$.BinderId')) = @BinderId
    and organization.Id = @organizationId
order by pane.Ordinal
    ,pane.Id