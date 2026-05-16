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
where (
        exists (
            select 1
            from [Content].[BinderPane-Active] binderPane
            where binderPane.PaneId = pane.Id
                and binderPane.BinderId = @BinderId
        )
        or exists (
            select 1
            from [Content].[CoursePane-Active] coursePane
                inner join [Content].[Course-Active] course on course.Id = coursePane.CourseId
            where coursePane.PaneId = pane.Id
                and coursePane.IdSource = 1
                and course.BinderId = @BinderId
        )
    )
    and organization.Id = @organizationId
order by case
        when exists (
            select 1
            from [Content].[BinderPane-Active] binderPane
            where binderPane.PaneId = pane.Id
                and binderPane.BinderId = @BinderId
        ) then 0
        else 1
    end
    ,pane.Ordinal
    ,pane.Id