create view [Content].[SurveyLicense-Active] as

select surveyLicense.Id as Id
    ,surveyLicense.SurveyId as SurveyId
    ,surveyLicense.LicenseId as LicenseId
from [Content].[SurveyLicense] surveyLicense
where 1=1
    and surveyLicense.IsDeleted = 0
    and surveyLicense.VersionOf = surveyLicense.Id