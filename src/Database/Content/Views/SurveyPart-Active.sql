create view [Content].[SurveyPart-Active] as

select surveyPart.Id as Id
    ,surveyPart.SurveyId as SurveyId
    ,surveyPart.Title as Title
    ,surveyPart.Instructions as Instructions
    ,surveyPart.Ordinal as Ordinal
from [Content].[SurveyPart] surveyPart
where 1=1
    and surveyPart.IsDeleted = 0
    and surveyPart.VersionOf = surveyPart.Id