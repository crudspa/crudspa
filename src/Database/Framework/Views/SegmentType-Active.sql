create view [Framework].[SegmentType-Active] as

select segmentType.Id as Id
    ,segmentType.Name as Name
    ,segmentType.EditorView as EditorView
    ,segmentType.DisplayView as DisplayView
    ,segmentType.Ordinal as Ordinal
from [Framework].[SegmentType] segmentType
where 1=1
    and segmentType.IsDeleted = 0