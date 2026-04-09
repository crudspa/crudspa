create proc [ContentDesign].[SectionSelectPageId] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@PageId uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @PageId = (
    select top 1 section.PageId
    from [Content].[Section-Active] section
        inner join [Content].[Page-Active] page on section.PageId = page.Id
        inner join [Framework].[Pane-Active] pane on
            try_convert(uniqueidentifier, json_value(pane.ConfigJson, '$.PageId')) = page.Id
            or try_convert(uniqueidentifier, json_value(pane.ConfigJson, '$.BinderId')) = page.BinderId
        inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    where section.Id = @Id
        and portal.OwnerId = @organizationId
    order by case
            when try_convert(uniqueidentifier, json_value(pane.ConfigJson, '$.PageId')) = page.Id then 0
            else 1
        end
        ,pane.Ordinal
        ,pane.Id
)