create trigger [Education].[ClassroomTeacherTrigger] on [Education].[ClassroomTeacher]
    for update
as

insert [Education].[ClassroomTeacher] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ClassroomId
    ,SchoolContactId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ClassroomId
    ,deleted.SchoolContactId
from deleted