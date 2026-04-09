create proc [EducationSchool].[VocabPartCompletedSelectForAssessmentAssignment] (
     @AssessmentAssignmentId uniqueidentifier
) as

select
    vocabPartCompleted.Id
    ,vocabPartCompleted.AssignmentId
    ,vocabPartCompleted.VocabPartId
    ,vocabPartCompleted.DeviceTimestamp
from [Education].[VocabPartCompleted-Active] vocabPartCompleted
where vocabPartCompleted.AssignmentId = @AssessmentAssignmentId