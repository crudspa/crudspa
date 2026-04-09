create proc [EducationSchool].[AssessmentAssignmentUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@StudentId uniqueidentifier
    ,@AssessmentId uniqueidentifier
    ,@StartAfter datetimeoffset(7)
    ,@EndBefore datetimeoffset(7)
) as

declare @schoolId uniqueidentifier = (
    select top 1 school.Id
    from [Education].[School-Active] school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        inner join [Framework].[User-Active] userTable on schoolContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,StudentId = @StudentId
    ,AssessmentId = @AssessmentId
    ,StartAfter = @StartAfter
    ,EndBefore = @EndBefore
from [Education].[AssessmentAssignment] baseTable
    inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.Id = baseTable.Id
    inner join [Education].[Student-Active] student on assessmentAssignment.StudentId = student.Id
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] school on family.SchoolId = school.Id
where baseTable.Id = @Id
    and school.Id = @schoolId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction