create proc [EducationPublisher].[AssessmentLicenseInsert] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
    ,@AssessmentId uniqueidentifier
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
    from [Education].[AssessmentLicense-Active] with (updlock, holdlock)
    where AssessmentId = @AssessmentId
        and LicenseId = @LicenseId
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

insert [Education].[AssessmentLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LicenseId
    ,AssessmentId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@LicenseId
    ,@AssessmentId
)

if not exists (
    select 1
    from [Education].[AssessmentLicense-Active] assessmentLicense
        inner join [Education].[Assessment-Active] assessment on assessmentLicense.AssessmentId = assessment.Id
        inner join [Framework].[License-Active] license on assessmentLicense.LicenseId = license.Id
    where assessmentLicense.Id = @Id
        and assessment.OwnerId = @organizationId
        and license.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction