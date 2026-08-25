create proc [EducationPublisher].[AssessmentLicenseSelectForLicense] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     assessmentLicense.Id
    ,assessmentLicense.LicenseId
    ,assessmentLicense.AssessmentId
    ,assessment.Name as AssessmentName
from [Education].[AssessmentLicense-Active] assessmentLicense
    inner join [Education].[Assessment-Active] assessment on assessmentLicense.AssessmentId = assessment.Id
    inner join [Framework].[License-Active] license on assessmentLicense.LicenseId = license.Id
where assessmentLicense.LicenseId = @LicenseId
    and assessment.OwnerId = @organizationId
    and license.OwnerId = @organizationId