create trigger [Content].[CommentTrigger] on [Content].[Comment]
    for update
as

insert [Content].[Comment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ParentId
    ,PostId
    ,ThreadId
    ,ById
    ,Posted
    ,Edited
    ,Body
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ParentId
    ,deleted.PostId
    ,deleted.ThreadId
    ,deleted.ById
    ,deleted.Posted
    ,deleted.Edited
    ,deleted.Body
from deleted