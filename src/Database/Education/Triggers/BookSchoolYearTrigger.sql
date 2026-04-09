create trigger [Education].[BookSchoolYearTrigger] on [Education].[BookSchoolYear]
    for update
as

insert [Education].[BookSchoolYear] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BookId
    ,SchoolYearId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BookId
    ,deleted.SchoolYearId
from deleted