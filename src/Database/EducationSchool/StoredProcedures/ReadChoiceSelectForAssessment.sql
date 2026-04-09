create proc [EducationSchool].[ReadChoiceSelectForAssessment] (
     @AssessmentId uniqueidentifier
) as

select
    readChoice.Id
    ,readChoice.ReadQuestionId
    ,readChoice.Text
    ,readChoice.IsCorrect
    ,readChoice.Ordinal
from [Education].[ReadChoice-Active] readChoice
    inner join [Education].[ReadQuestion] readQuestion on readChoice.ReadQuestionId = readQuestion.Id
    inner join [Education].[ReadPart] readPart on readQuestion.ReadPartId = readPart.Id
    inner join [Education].[Assessment] assessment on readPart.AssessmentId = assessment.Id
where assessment.Id = @AssessmentId