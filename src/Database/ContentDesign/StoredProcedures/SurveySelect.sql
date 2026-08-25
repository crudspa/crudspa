create proc [ContentDesign].[SurveySelect] (
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
     survey.Id
    ,survey.PortalId
    ,survey.Title
from [Content].[Survey-Active] survey
    inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where survey.Id = @Id
    and organization.Id = @organizationId

select distinct
     @Id as SurveyId
    ,license.Id as LicenseId
    ,license.Name as LicenseName
    ,convert(bit, iif(surveyLicense.Id is null, 0, 1)) as Selected
from [Framework].[License-Active] license
    left join [Content].[SurveyLicense-Active] surveyLicense on surveyLicense.LicenseId = license.Id
        and surveyLicense.SurveyId = @Id
where license.OwnerId = @organizationId
order by license.Name