create view [Education].[ClassroomTeacher-Active] as

select classroomTeacher.Id as Id
    ,classroomTeacher.ClassroomId as ClassroomId
    ,classroomTeacher.SchoolContactId as SchoolContactId
from [Education].[ClassroomTeacher] classroomTeacher
where 1=1
    and classroomTeacher.IsDeleted = 0
    and classroomTeacher.VersionOf = classroomTeacher.Id