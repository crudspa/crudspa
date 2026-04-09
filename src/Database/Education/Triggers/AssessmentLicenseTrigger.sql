create trigger [Education].[AssessmentLicenseTrigger] on [Education].[AssessmentLicense]
    for update
as

insert [Education].[AssessmentLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AssessmentId
    ,LicenseId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.AssessmentId
    ,deleted.LicenseId
from deleted