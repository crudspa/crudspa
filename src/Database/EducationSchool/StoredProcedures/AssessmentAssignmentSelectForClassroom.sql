create proc [EducationSchool].[AssessmentAssignmentSelectForClassroom] (
     @SessionId uniqueidentifier
    ,@AssessmentId uniqueidentifier
    ,@ClassroomId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)
declare @schoolId uniqueidentifier = (
    select top 1 school.Id
    from [Education].[School-Active] school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        inner join [Framework].[User-Active] userTable on schoolContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

select distinct
    assessmentAssignment.Id as Id
    ,assessmentAssignment.AssessmentId as AssessmentId
    ,assessmentAssignment.StudentId as StudentId
from [Education].[AssessmentAssignment-Active] assessmentAssignment
    inner join [Education].[Assessment-Active] assessment on assessmentAssignment.AssessmentId = assessment.Id
    inner join [Education].[Student-Active] student on assessmentAssignment.StudentId = student.Id
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] school on family.SchoolId = school.Id
    inner join [Education].[Classroom-Active] classroom on classroom.SchoolId = school.Id
where assessmentAssignment.AssessmentId = @AssessmentId
    and assessmentAssignment.Terminated is null
    and assessmentAssignment.Completed is null
    and student.DeletedBySchool = 0
    and student.Id in (select StudentId from [Education].[StudentSchoolYear-Active] where SchoolYearId = @schoolYearId)
    and classroom.SchoolYearId = @schoolYearId
    and classroom.Id = @ClassroomId
    and classroom.SchoolId = @schoolId
    and student.Id in (
        select StudentId
        from [Education].[ClassroomStudent-Active]
        where ClassroomId = @ClassroomId
    )