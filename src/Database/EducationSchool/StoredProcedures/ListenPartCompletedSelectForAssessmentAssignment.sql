create proc [EducationSchool].[ListenPartCompletedSelectForAssessmentAssignment] (
     @AssessmentAssignmentId uniqueidentifier
) as

select
    listenPartCompleted.Id
    ,listenPartCompleted.AssignmentId
    ,listenPartCompleted.ListenPartId
    ,listenPartCompleted.DeviceTimestamp
from [Education].[ListenPartCompleted-Active] listenPartCompleted
where listenPartCompleted.AssignmentId = @AssessmentAssignmentId