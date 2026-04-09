create view [Education].[AppSurveyResponse-Active] as

select appSurveyResponse.Id as Id
    ,appSurveyResponse.AssignmentBatchId as AssignmentBatchId
    ,appSurveyResponse.QuestionId as QuestionId
    ,appSurveyResponse.AnswerId as AnswerId
from [Education].[AppSurveyResponse] appSurveyResponse
where 1=1
    and appSurveyResponse.IsDeleted = 0
    and appSurveyResponse.VersionOf = appSurveyResponse.Id