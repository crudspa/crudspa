create proc [EducationSchool].[ClassroomSelectableSchoolContacts] (
     @SessionId uniqueidentifier
    ,@ClassroomId uniqueidentifier
) as

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
    ,schoolContact.Id as Id
    ,trim(contact.FirstName + ' ' + contact.LastName) as Name
    ,convert(bit, case when classroomTeacher.Id is null then 0 else 1 end) as Selected
from [Education].[SchoolContact-Active] schoolContact
    inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
    left join [Education].[ClassroomTeacher-Active] classroomTeacher on classroomTeacher.SchoolContactId = schoolContact.Id
        and classroomTeacher.ClassroomId = @ClassroomId
where schoolContact.SchoolId = @schoolId
order by trim(contact.FirstName + ' ' + contact.LastName)