create proc [EducationPublisher].[SurveyLicenseSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     surveyLicense.Id
    ,surveyLicense.LicenseId
    ,surveyLicense.SurveyId
    ,survey.Title as SurveyTitle
from [Content].[SurveyLicense-Active] surveyLicense
    inner join [Framework].[License-Active] license on surveyLicense.LicenseId = license.Id
    inner join [Content].[Survey-Active] survey on surveyLicense.SurveyId = survey.Id
    inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
where surveyLicense.Id = @Id
    and portal.OwnerId = @organizationId
    and license.OwnerId = @organizationId