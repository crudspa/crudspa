create trigger [Content].[CommentTagTrigger] on [Content].[CommentTag]
    for update
as

insert [Content].[CommentTag] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,CommentId
    ,TagId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.CommentId
    ,deleted.TagId
from deleted