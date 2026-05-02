create proc [ContentDisplay].[SeoRouteSelect] (
     @PortalId uniqueidentifier
) as

set nocount on

;with route as (
    select
         segment.Id
        ,segment.ParentId
        ,cast('/' + segment.[Key] as nvarchar(2048)) as [Path]
        ,segment.Title
        ,segment.SeoDescription
        ,segment.Ordinal
    from [Framework].[Segment-Active] segment
        inner join [Framework].[ContentStatus-Active] status on segment.StatusId = status.Id
    where segment.PortalId = @PortalId
        and segment.ParentId is null
        and segment.PermissionId is null
        and segment.AllLicenses = 1
        and segment.Fixed = 1
        and segment.RequiresId = 0
        and status.Name = 'Complete'

    union all

    select
         child.Id
        ,child.ParentId
        ,cast(route.[Path] + '/' + child.[Key] as nvarchar(2048)) as [Path]
        ,child.Title
        ,child.SeoDescription
        ,child.Ordinal
    from [Framework].[Segment-Active] child
        inner join route on child.ParentId = route.Id
        inner join [Framework].[ContentStatus-Active] status on child.StatusId = status.Id
    where child.PortalId = @PortalId
        and child.PermissionId is null
        and child.AllLicenses = 1
        and child.Fixed = 1
        and child.RequiresId = 0
        and status.Name = 'Complete'
)
,defaultSegment as (
    select top 1 segment.Id
    from [Framework].[Segment-Active] segment
        inner join [Framework].[ContentStatus-Active] status on segment.StatusId = status.Id
    where segment.PortalId = @PortalId
        and segment.ParentId is null
        and segment.PermissionId is null
        and segment.AllLicenses = 1
        and segment.Fixed = 1
        and segment.RequiresId = 0
        and status.Name = 'Complete'
    order by segment.Ordinal, segment.[Key]
)
select
     route.[Path]
    ,route.Title
    ,pagePane.PageId
    ,page.Title as PageTitle
    ,route.SeoDescription
    ,convert(bit, iif(route.Id = defaultSegment.Id, 1, 0)) as IsDefault
from route
    cross join defaultSegment
    outer apply (
        select top 1 try_convert(uniqueidentifier, json_value(pane.ConfigJson, '$.PageId')) as PageId
        from [Framework].[Pane-Active] pane
            inner join [Framework].[PaneType-Active] paneType on pane.TypeId = paneType.Id
        where pane.SegmentId = route.Id
            and pane.PermissionId is null
            and paneType.DisplayView like 'Crudspa.Content.Display.Client.Plugins.PaneType.PageDisplay,%'
        order by pane.Ordinal
    ) pagePane
    left join [Content].[Page-Active] page on pagePane.PageId = page.Id
    left join [Framework].[ContentStatus-Active] pageStatus on page.StatusId = pageStatus.Id
where pagePane.PageId is null
    or pageStatus.Name = 'Complete'
order by route.[Path]