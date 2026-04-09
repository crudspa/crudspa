create trigger [Samples].[BookTagTrigger] on [Samples].[BookTag]
    for update
as

insert [Samples].[BookTag] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BookId
    ,TagId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BookId
    ,deleted.TagId
from deleted