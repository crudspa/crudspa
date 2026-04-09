create trigger [Education].[BookActivityTrigger] on [Education].[BookActivity]
    for update
as

insert [Education].[BookActivity] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BookId
    ,ActivityId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BookId
    ,deleted.ActivityId
from deleted