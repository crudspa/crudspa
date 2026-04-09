create proc [FrameworkCore].[SegmentTypeSelectFull] (
     @PortalId uniqueidentifier
) as

select
    segmentType.Id
    ,segmentType.Name
    ,segmentType.EditorView
    ,segmentType.DisplayView
    ,segmentType.Ordinal
from [Framework].[SegmentType-Active] segmentType
    inner join [Framework].[PortalSegmentType-Active] portalSegmentType on portalSegmentType.TypeId = segmentType.Id
where portalSegmentType.PortalId = @PortalId
order by segmentType.Ordinal