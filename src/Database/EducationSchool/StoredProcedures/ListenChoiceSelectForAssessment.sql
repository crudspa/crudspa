create proc [EducationSchool].[ListenChoiceSelectForAssessment] (
     @AssessmentId uniqueidentifier
) as

select
    listenChoice.Id
    ,listenChoice.ListenQuestionId
    ,listenChoice.Text
    ,listenChoice.IsCorrect
    ,listenChoice.Ordinal as Ordinal
from [Education].[ListenChoice-Active] listenChoice
    inner join [Education].[ListenQuestion] listenQuestion on listenChoice.ListenQuestionId = listenQuestion.Id
    inner join [Education].[ListenPart] listenPart on listenQuestion.ListenPartId = listenPart.Id
    inner join [Education].[Assessment] assessment on listenPart.AssessmentId = assessment.Id
where assessment.Id = @AssessmentId