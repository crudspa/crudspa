create trigger [Education].[ClassroomTrigger] on [Education].[Classroom]
    for update
as

insert [Education].[Classroom] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OrganizationId
    ,SchoolId
    ,GradeId
    ,SchoolYearId
    ,TypeId
    ,ImportNum
    ,SmallClassroom
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OrganizationId
    ,deleted.SchoolId
    ,deleted.GradeId
    ,deleted.SchoolYearId
    ,deleted.TypeId
    ,deleted.ImportNum
    ,deleted.SmallClassroom
from deleted