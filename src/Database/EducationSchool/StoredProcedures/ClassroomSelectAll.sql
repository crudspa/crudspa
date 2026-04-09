create proc [EducationSchool].[ClassroomSelectAll] (
     @SessionId uniqueidentifier
) as

declare @currentSchoolYear uniqueidentifier = (select top 1 Id from [Education].[SchoolYear-Active] where getdate() between Starts and Ends)

declare @schoolId uniqueidentifier = (
    select top 1 schoolContact.SchoolId
    from [Framework].[Session-Active] session
        inner join [Framework].[User-Active] userTable on session.UserId = userTable.Id
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.UserId = userTable.Id
    where session.Id = @SessionId

)

select
    classroom.Id
    ,classroom.TypeId
    ,classroom.GradeId
    ,organization.Name as OrganizationName
    ,type.Name as TypeName
    ,grade.Name as GradeName
    ,(select count(1) from [Education].[ClassroomStudent-Active] classroomStudent where classroomStudent.ClassroomId = classroom.Id) as ClassroomStudentCount
    ,classroom.SmallClassroom
from [Education].[Classroom-Active] classroom
    inner join [Framework].[Organization-Active] organization on classroom.OrganizationId = organization.Id
    inner join [Education].[ClassroomType-Active] type on classroom.TypeId = type.Id
    inner join [Education].[Grade-Active] grade on classroom.GradeId = grade.Id
where classroom.SchoolId = @schoolId
    and classroom.SchoolYearId = @currentSchoolYear


select
    classroomTeacher.Id
    ,classroomTeacher.ClassroomId
    ,classroomTeacher.SchoolContactId
    ,trim(contact.FirstName  + ' ' + contact.LastName) as Name
from [Education].[ClassroomTeacher-Active] classroomTeacher
    inner join [Education].[SchoolContact-Active] schoolContact on classroomTeacher.SchoolContactId = schoolContact.Id
    inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
    inner join [Education].[Classroom-Active] classroom on classroomTeacher.ClassroomId = classroom.Id
where classroom.SchoolId = @schoolId
    and classroom.SchoolYearId = @currentSchoolYear