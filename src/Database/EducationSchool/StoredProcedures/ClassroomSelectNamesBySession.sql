create proc [EducationSchool].[ClassroomSelectNamesBySession] (
     @SessionId uniqueidentifier
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

select distinct
    classroom.Id as Id
    ,organization.Name as Name
from [Education].[Classroom-Active] classroom
    inner join [Framework].[Organization-Active] organization on classroom.OrganizationId = organization.Id
    inner join [Education].[ClassroomTeacher-Active] classroomTeacher on classroom.Id = classroomTeacher.ClassroomId
where classroom.SchoolId = @schoolId
    and classroom.SchoolYearId = @schoolYearId
order by organization.Name