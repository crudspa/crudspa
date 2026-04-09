create proc [EducationSchool].[AssessmentAssignmentInsert] (
     @SessionId uniqueidentifier
    ,@StudentId uniqueidentifier
    ,@AssessmentId uniqueidentifier
    ,@StartAfter datetimeoffset(7)
    ,@EndBefore datetimeoffset(7)
    ,@Id uniqueidentifier output
) as

declare @schoolId uniqueidentifier = (
    select top 1 school.Id
    from [Education].[School-Active] school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        inner join [Framework].[User-Active] userTable on schoolContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[AssessmentAssignment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,StudentId
    ,AssessmentId
    ,StartAfter
    ,EndBefore
    ,Assigned
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@StudentId
    ,@AssessmentId
    ,@StartAfter
    ,@EndBefore
    ,@now
)

if not exists (
    select 1
    from [Education].[AssessmentAssignment-Active] assessmentAssignment
        inner join [Education].[Student-Active] student on assessmentAssignment.StudentId = student.Id
        inner join [Education].[Family-Active] family on student.FamilyId = family.Id
        inner join [Education].[School-Active] school on family.SchoolId = school.Id
    where assessmentAssignment.Id = @Id
        and school.Id = @schoolId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction