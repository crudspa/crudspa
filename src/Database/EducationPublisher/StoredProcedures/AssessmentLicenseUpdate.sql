create proc [EducationPublisher].[AssessmentLicenseUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@AssessmentId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if exists (
    select 1
    from [Education].[AssessmentLicense-Active] with (updlock, holdlock)
    where Id != @Id
        and AssessmentId = @AssessmentId
        and LicenseId = (select LicenseId from [Education].[AssessmentLicense-Active] where Id = @Id)
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,AssessmentId = @AssessmentId
from [Education].[AssessmentLicense] baseTable
    inner join [Education].[AssessmentLicense-Active] assessmentLicense on assessmentLicense.Id = baseTable.Id
    inner join [Education].[Assessment-Active] assessment on assessmentLicense.AssessmentId = assessment.Id
    inner join [Framework].[License-Active] license on assessmentLicense.LicenseId = license.Id
where baseTable.Id = @Id
    and assessment.OwnerId = @organizationId
    and license.OwnerId = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction