create view [Education].[ActivityAssignment-Active] as

select activityAssignment.Id as Id
    ,activityAssignment.AssignmentBatchId as AssignmentBatchId
    ,activityAssignment.ActivityId as ActivityId
    ,activityAssignment.Started as Started
    ,activityAssignment.Finished as Finished
    ,activityAssignment.StatusId as StatusId
    ,activityAssignment.Ordinal as Ordinal
from [Education].[ActivityAssignment] activityAssignment
where 1=1
    and activityAssignment.IsDeleted = 0
    and activityAssignment.VersionOf = activityAssignment.Id