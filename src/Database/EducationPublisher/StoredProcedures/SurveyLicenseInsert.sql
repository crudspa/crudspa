create proc [EducationPublisher].[SurveyLicenseInsert] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
    ,@SurveyId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if exists (
    select 1
    from [Content].[SurveyLicense-Active] with (updlock, holdlock)
    where SurveyId = @SurveyId
        and LicenseId = @LicenseId
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

insert [Content].[SurveyLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LicenseId
    ,SurveyId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@LicenseId
    ,@SurveyId
)

if not exists (
    select 1
    from [Content].[SurveyLicense-Active] surveyLicense
        inner join [Framework].[License-Active] license on surveyLicense.LicenseId = license.Id
        inner join [Content].[Survey-Active] survey on surveyLicense.SurveyId = survey.Id
        inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
    where surveyLicense.Id = @Id
        and portal.OwnerId = @organizationId
        and license.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction