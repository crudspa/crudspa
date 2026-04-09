create proc [EducationStudent].[GameActivitySelectForBatch] (
     @AssignmentBatchId uniqueidentifier
) as

set nocount on

select
     activityAssignment.Id
    ,activityAssignment.AssignmentBatchId
    ,activityAssignment.ActivityId
    ,activityAssignment.Ordinal
    ,activityAssignment.Started
    ,activityAssignment.Finished
    ,activityAssignment.StatusId
into #AssignedActivities
from [Education].[ActivityAssignment-Active] activityAssignment
where activityAssignment.AssignmentBatchId = @AssignmentBatchId

select distinct gameActivityView.*
    ,assignedActivities.AssignmentBatchId as ActivityAssignmentAssignmentBatchId
    ,assignedActivities.Ordinal as ActivityAssignmentOrdinal
    ,assignedActivities.Id as ActivityAssignmentId
    ,assignmentBatch.Published as AssignmentBatchAssigned
    ,assignedActivities.Started as ActivityAssignmentStarted
    ,assignedActivities.Finished as ActivityAssignmentFinished
    ,assignedActivities.StatusId as ActivityAssignmentStatusId
    ,activityAssignmentStatus.Name as ActivityAssignmentStatusName
from [Education].[AssignmentBatch-Active] assignmentBatch
    inner join #AssignedActivities assignedActivities on assignedActivities.AssignmentBatchId = assignmentBatch.Id
    left join [Education].[ActivityAssignmentStatus-Active] activityAssignmentStatus on assignedActivities.StatusId = activityAssignmentStatus.Id
    inner join [EducationStudent].[GameActivity-Deep] gameActivityView on assignedActivities.ActivityId = gameActivityView.GameActivityActivityId
where assignmentBatch.Id = @AssignmentBatchId
    and gameActivityView.GameId = assignmentBatch.GameId
order by assignedActivities.Ordinal

select distinct activityChoice.*
from [Education].[ActivityChoice-Deep] activityChoice
    inner join #AssignedActivities assignedActivities on activityChoice.ActivityId = assignedActivities.ActivityId