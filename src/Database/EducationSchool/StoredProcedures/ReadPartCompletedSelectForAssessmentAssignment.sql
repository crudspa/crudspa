create proc [EducationSchool].[ReadPartCompletedSelectForAssessmentAssignment] (
     @AssessmentAssignmentId uniqueidentifier
) as

select
    readPartCompleted.Id
    ,readPartCompleted.AssignmentId
    ,readPartCompleted.ReadPartId
    ,readPartCompleted.DeviceTimestamp
from [Education].[ReadPartCompleted-Active] readPartCompleted
where readPartCompleted.AssignmentId = @AssessmentAssignmentId