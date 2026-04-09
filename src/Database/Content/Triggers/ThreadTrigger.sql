create trigger [Content].[ThreadTrigger] on [Content].[Thread]
    for update
as

insert [Content].[Thread] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ForumId
    ,Title
    ,CommentId
    ,Pinned
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ForumId
    ,deleted.Title
    ,deleted.CommentId
    ,deleted.Pinned
from deleted