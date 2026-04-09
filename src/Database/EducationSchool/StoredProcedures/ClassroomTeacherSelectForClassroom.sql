create proc [EducationSchool].[ClassroomTeacherSelectForClassroom] (
     @ClassroomId uniqueidentifier
) as

select
    classroomTeacher.Id
    ,classroomTeacher.ClassroomId
    ,classroomTeacher.SchoolContactId
    ,trim(contact.FirstName  + ' ' + contact.LastName) as Name
from [Education].[ClassroomTeacher-Active] classroomTeacher
    inner join [Education].[SchoolContact-Active] schoolContact on classroomTeacher.SchoolContactId = schoolContact.Id
    inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
    inner join [Education].[Classroom-Active] classroom on classroomTeacher.ClassroomId = classroom.Id
where classroomTeacher.ClassroomId = @ClassroomId