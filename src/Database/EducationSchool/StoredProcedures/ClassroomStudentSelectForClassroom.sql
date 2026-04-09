create proc [EducationSchool].[ClassroomStudentSelectForClassroom] (
     @ClassroomId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)

select
    classroomStudent.Id as Id
    ,classroomStudent.ClassroomId as ClassroomId
    ,classroomStudent.StudentId as StudentId
    ,trim(contact.FirstName + ' ' + contact.LastName) as Name
    ,student.SecretCode
    ,student.IsTestAccount
from [Education].[ClassroomStudent-Active] classroomStudent
    inner join [Education].[Student-Active] student on classroomStudent.StudentId = student.Id
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
where classroomStudent.ClassroomId = @ClassroomId
    and student.DeletedBySchool = 0
    and student.Id in (select StudentId from [Education].[StudentSchoolYear-Active] where SchoolYearId = @schoolYearId)
order by contact.FirstName, contact.LastName