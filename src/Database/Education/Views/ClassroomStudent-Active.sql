create view [Education].[ClassroomStudent-Active] as

select classroomStudent.Id as Id
    ,classroomStudent.ClassroomId as ClassroomId
    ,classroomStudent.StudentId as StudentId
from [Education].[ClassroomStudent] classroomStudent
where 1=1
    and classroomStudent.IsDeleted = 0
    and classroomStudent.VersionOf = classroomStudent.Id