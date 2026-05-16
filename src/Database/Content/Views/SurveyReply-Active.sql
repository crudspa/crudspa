create view [Content].[SurveyReply-Active] as

select surveyReply.Id as Id
    ,surveyReply.SurveyId as SurveyId
    ,surveyReply.BinderId as BinderId
    ,surveyReply.ContactId as ContactId
    ,surveyReply.Started as Started
    ,surveyReply.Completed as Completed
    ,surveyReply.Terminated as Terminated
from [Content].[SurveyReply] surveyReply
where 1=1
    and surveyReply.IsDeleted = 0
    and surveyReply.VersionOf = surveyReply.Id