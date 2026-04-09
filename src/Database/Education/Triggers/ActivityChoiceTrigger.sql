create trigger [Education].[ActivityChoiceTrigger] on [Education].[ActivityChoice]
    for update
as

insert [Education].[ActivityChoice] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ActivityId
    ,Text
    ,AudioFileId
    ,ImageFileId
    ,IsCorrect
    ,ColumnOrdinal
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ActivityId
    ,deleted.Text
    ,deleted.AudioFileId
    ,deleted.ImageFileId
    ,deleted.IsCorrect
    ,deleted.ColumnOrdinal
    ,deleted.Ordinal
from deleted