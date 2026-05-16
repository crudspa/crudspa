create view [Content].[SurveyQuestion-Active] as

select surveyQuestion.Id as Id
    ,surveyQuestion.PartId as PartId
    ,surveyQuestion.QuestionId as QuestionId
    ,surveyQuestion.Ordinal as Ordinal
from [Content].[SurveyQuestion] surveyQuestion
where 1=1
    and surveyQuestion.IsDeleted = 0
    and surveyQuestion.VersionOf = surveyQuestion.Id