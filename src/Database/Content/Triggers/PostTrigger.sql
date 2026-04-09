create trigger [Content].[PostTrigger] on [Content].[Post]
    for update
as

insert [Content].[Post] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BlogId
    ,PageId
    ,StatusId
    ,Title
    ,Author
    ,Published
    ,Revised
    ,CommentRule
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BlogId
    ,deleted.PageId
    ,deleted.StatusId
    ,deleted.Title
    ,deleted.Author
    ,deleted.Published
    ,deleted.Revised
    ,deleted.CommentRule
from deleted