create proc [ContentDesign].[BinderPaneSelectForPane] (
     @SessionId uniqueidentifier
    ,@PaneId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select top 1
     binderPane.Id
    ,binderPane.PaneId
    ,binderPane.BinderId
from [Content].[BinderPane-Active] binderPane
    inner join [Framework].[Pane-Active] pane on binderPane.PaneId = pane.Id
    inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where binderPane.PaneId = @PaneId
    and organization.Id = @organizationId
order by binderPane.Id