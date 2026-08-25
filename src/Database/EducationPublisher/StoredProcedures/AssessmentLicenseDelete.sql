create proc [EducationPublisher].[AssessmentLicenseDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
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