create proc [EducationSchool].[StudentSelectForSchool] (
     @SessionId uniqueidentifier
    ,@AssessmentId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)

declare @schoolId uniqueidentifier = (
    select top 1 school.Id
    from [Education].[School-Active] school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @gradeId uniqueidentifier = (select GradeId from [Education].[Assessment-Active] where Id = @AssessmentId)

select distinct
    student.Id
from [Education].[Student-Active] student
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] school on family.SchoolId = school.Id
where student.DeletedBySchool = 0
    and student.Id in (select StudentId from [Education].[StudentSchoolYear-Active] where SchoolYearId = @schoolYearId)
    and student.IsTestAccount = 0
    and student.GradeId = @gradeId
    and school.Id = @schoolId