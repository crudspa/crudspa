create proc [EducationSchool].[ClassroomSelectableStudents] (
     @SessionId uniqueidentifier
    ,@ClassroomId uniqueidentifier
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
select
     @ClassroomId as RootId
    ,student.Id as Id
    ,trim(contact.FirstName + ' ' + contact.LastName) as Name
    ,convert(bit, case when classroomStudent.Id is null then 0 else 1 end) as Selected
from [Education].[Student-Active] student
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    left join [Education].[ClassroomStudent-Active] classroomStudent on classroomStudent.StudentId = student.Id
        and classroomStudent.ClassroomId = @ClassroomId
where family.SchoolId = @schoolId
    and student.DeletedBySchool = 0
    and student.Id in (select StudentId from [Education].[StudentSchoolYear-Active] where SchoolYearId = @schoolYearId)
    and student.IsTestAccount = 0
order by contact.FirstName, contact.LastName