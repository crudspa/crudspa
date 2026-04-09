create trigger [Education].[ClassroomStudentTrigger] on [Education].[ClassroomStudent]
    for update
as

insert [Education].[ClassroomStudent] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ClassroomId
    ,StudentId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ClassroomId
    ,deleted.StudentId
from deleted