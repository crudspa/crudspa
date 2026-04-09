create proc [EducationStudent].[ActivityAssignmentSelectPrevious] (
     @StudentId uniqueidentifier
    ,@GameId uniqueidentifier
) as

select distinct
    activityAssignment.ActivityId as ActivityId
    ,activity.[Key] as ActivityKey
from [Education].[ActivityAssignment-Active] activityAssignment
    inner join [Education].[Activity-Active] activity on activityAssignment.ActivityId = activity.Id
    inner join [Education].[AssignmentBatch-Active] assignmentBatch on activityAssignment.AssignmentBatchId = assignmentBatch.Id
where assignmentBatch.StudentId = @StudentId
    and assignmentBatch.GameId = @GameId