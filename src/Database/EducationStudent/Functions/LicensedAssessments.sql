create function [EducationStudent].[LicensedAssessments] (
    @SessionId uniqueidentifier
)
returns table
as
return
(
    select assessment.Id as AssessmentId
    from [Education].[Assessment-Active] assessment
    where not exists (
        select 1
        from [Education].[AssessmentLicense-Active] assessmentLicense
        where assessmentLicense.AssessmentId = assessment.Id
    )

    union

    select assessmentLicense.AssessmentId
    from [Education].[AssessmentLicense-Active] assessmentLicense
        inner join [EducationCommon].[SessionLicenses](@SessionId) sessionLicense
            on sessionLicense.LicenseId = assessmentLicense.LicenseId
    where assessmentLicense.AssessmentId is not null
);