create proc [EducationSchool].[ClassroomSelect] (
     @Id uniqueidentifier
) as

select
    classroom.Id as Id
    ,classroom.TypeId as TypeId
    ,grade.Id as GradeId
    ,organization.Name as OrganizationName
    ,type.Name as TypeName
    ,grade.Name as GradeName
    ,(select count(1) from [Education].[ClassroomStudent-Active] classroomStudent where classroomStudent.ClassroomId = classroom.Id) as ClassroomStudentCount
    ,classroom.SmallClassroom
from [Education].[Classroom-Active] classroom
    inner join [Framework].[Organization-Active] organization on classroom.OrganizationId = organization.Id
    inner join [Education].[ClassroomType-Active] type on classroom.TypeId = type.Id
    inner join [Education].[Grade-Active] grade on classroom.GradeId = grade.Id
where classroom.Id = @Id