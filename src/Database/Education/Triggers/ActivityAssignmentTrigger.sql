create trigger [Education].[ActivityAssignmentTrigger] on [Education].[ActivityAssignment]
    for update
as

insert [Education].[ActivityAssignment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AssignmentBatchId
    ,ActivityId
    ,Started
    ,Finished
    ,StatusId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.AssignmentBatchId
    ,deleted.ActivityId
    ,deleted.Started
    ,deleted.Finished
    ,deleted.StatusId
    ,deleted.Ordinal
from deleted