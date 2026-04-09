create trigger [Content].[ThreadTagTrigger] on [Content].[ThreadTag]
    for update
as

insert [Content].[ThreadTag] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ThreadId
    ,TagId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ThreadId
    ,deleted.TagId
from deleted