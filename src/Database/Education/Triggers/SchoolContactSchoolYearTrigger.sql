create trigger [Education].[SchoolContactSchoolYearTrigger] on [Education].[SchoolContactSchoolYear]
    for update
as

insert [Education].[SchoolContactSchoolYear] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,SchoolContactId
    ,SchoolYearId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.SchoolContactId
    ,deleted.SchoolYearId
from deleted