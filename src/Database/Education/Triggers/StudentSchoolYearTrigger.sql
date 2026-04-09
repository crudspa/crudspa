create trigger [Education].[StudentSchoolYearTrigger] on [Education].[StudentSchoolYear]
    for update
as

insert [Education].[StudentSchoolYear] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,StudentId
    ,SchoolYearId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.StudentId
    ,deleted.SchoolYearId
from deleted