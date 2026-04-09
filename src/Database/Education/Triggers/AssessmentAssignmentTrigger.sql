create trigger [Education].[AssessmentAssignmentTrigger] on [Education].[AssessmentAssignment]
    for update
as

insert [Education].[AssessmentAssignment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AssessmentId
    ,StudentId
    ,Assigned
    ,StartAfter
    ,EndBefore
    ,Started
    ,Completed
    ,Terminated
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.AssessmentId
    ,deleted.StudentId
    ,deleted.Assigned
    ,deleted.StartAfter
    ,deleted.EndBefore
    ,deleted.Started
    ,deleted.Completed
    ,deleted.Terminated
from deleted