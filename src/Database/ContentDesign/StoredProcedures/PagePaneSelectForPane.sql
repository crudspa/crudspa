create proc [ContentDesign].[PagePaneSelectForPane] (
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
     pagePane.Id
    ,pagePane.PaneId
    ,pagePane.PageId
from [Content].[PagePane-Active] pagePane
    inner join [Framework].[Pane-Active] pane on pagePane.PaneId = pane.Id
    inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where pagePane.PaneId = @PaneId
    and organization.Id = @organizationId
order by pagePane.Id