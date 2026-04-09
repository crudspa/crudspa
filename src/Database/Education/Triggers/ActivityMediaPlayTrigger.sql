create trigger [Education].[ActivityMediaPlayTrigger] on [Education].[ActivityMediaPlay]
    for update
as

insert [Education].[ActivityMediaPlay] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,MediaPlayId
    ,AssignmentBatchId
    ,ActivityId
    ,ActivityChoiceId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.MediaPlayId
    ,deleted.AssignmentBatchId
    ,deleted.ActivityId
    ,deleted.ActivityChoiceId
from deleted