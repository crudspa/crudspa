create proc [EducationDistrict].[AssessmentAssignmentDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @districtId uniqueidentifier = (
    select top 1 district.Id
    from [Education].[District-Active] district
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.DistrictId = district.Id
        inner join [Framework].[User-Active] userTable on districtContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[AssessmentAssignment] baseTable
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.Id = baseTable.Id
    inner join [Education].[Student-Active] student on assessmentAssignment.StudentId = student.Id
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] school on family.SchoolId = school.Id
where baseTable.Id = @Id
    and school.DistrictId = @districtId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction