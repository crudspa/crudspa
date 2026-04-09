create view [Education].[AssessmentLicense-Active] as

select assessmentLicense.Id as Id
    ,assessmentLicense.AssessmentId as AssessmentId
    ,assessmentLicense.LicenseId as LicenseId
from [Education].[AssessmentLicense] assessmentLicense
where 1=1
    and assessmentLicense.IsDeleted = 0
    and assessmentLicense.VersionOf = assessmentLicense.Id