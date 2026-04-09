create trigger [Content].[PostTagTrigger] on [Content].[PostTag]
    for update
as

insert [Content].[PostTag] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PostId
    ,TagId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PostId
    ,deleted.TagId
from deleted