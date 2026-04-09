create view [Framework].[Pane-Active] as

select pane.Id as Id
    ,pane.[Key] as [Key]
    ,pane.Title as Title
    ,pane.SegmentId as SegmentId
    ,pane.TypeId as TypeId
    ,pane.PermissionId as PermissionId
    ,pane.ConfigJson as ConfigJson
    ,pane.Ordinal as Ordinal
from [Framework].[Pane] pane
where 1=1
    and pane.IsDeleted = 0
    and pane.VersionOf = pane.Id