create view [Content].[Post-Active] as

select post.Id as Id
    ,post.BlogId as BlogId
    ,post.PageId as PageId
    ,post.StatusId as StatusId
    ,post.Title as Title
    ,post.Author as Author
    ,post.Published as Published
    ,post.Revised as Revised
    ,post.CommentRule as CommentRule
from [Content].[Post] post
where 1=1
    and post.IsDeleted = 0
    and post.VersionOf = post.Id