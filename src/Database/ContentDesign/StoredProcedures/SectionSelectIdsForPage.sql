create proc [ContentDesign].[SectionSelectIdsForPage] (
     @SessionId uniqueidentifier
    ,@PageId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select section.Id
from [Content].[Section-Active] section
    inner join [Content].[Page-Active] page on section.PageId = page.Id
    inner join [Framework].[Pane-Active] pane on
        try_convert(uniqueidentifier, json_value(pane.ConfigJson, '$.PageId')) = page.Id
        or try_convert(uniqueidentifier, json_value(pane.ConfigJson, '$.BinderId')) = page.BinderId
    inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
where section.PageId = @PageId
    and portal.OwnerId = @organizationId
order by section.Ordinal