create trigger [Education].[StudentBookTrigger] on [Education].[StudentBook]
    for update
as

insert [Education].[StudentBook] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,StudentId
    ,BookId
    ,SchoolYearId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.StudentId
    ,deleted.BookId
    ,deleted.SchoolYearId
from deleted