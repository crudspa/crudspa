create trigger [Framework].[MediaPlayTrigger] on [Framework].[MediaPlay]
    for update
as

insert [Framework].[MediaPlay] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AudioFileId
    ,VideoFileId
    ,Started
    ,Canceled
    ,Completed
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.AudioFileId
    ,deleted.VideoFileId
    ,deleted.Started
    ,deleted.Canceled
    ,deleted.Completed
from deleted