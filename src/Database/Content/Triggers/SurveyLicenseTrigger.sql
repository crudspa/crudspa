create trigger [Content].[SurveyLicenseTrigger] on [Content].[SurveyLicense]
    for update
as

insert [Content].[SurveyLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,SurveyId
    ,LicenseId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.SurveyId
    ,deleted.LicenseId
from deleted