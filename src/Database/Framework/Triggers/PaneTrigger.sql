create trigger [Framework].[PaneTrigger] on [Framework].[Pane]
    for update
as

insert [Framework].[Pane] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,[Key]
    ,Title
    ,SegmentId
    ,TypeId
    ,PermissionId
    ,ConfigJson
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.[Key]
    ,deleted.Title
    ,deleted.SegmentId
    ,deleted.TypeId
    ,deleted.PermissionId
    ,deleted.ConfigJson
    ,deleted.Ordinal
from deleted