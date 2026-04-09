create view [Content].[Thread-Active] as

select thread.Id as Id
    ,thread.ForumId as ForumId
    ,thread.Title as Title
    ,thread.CommentId as CommentId
    ,thread.Pinned as Pinned
from [Content].[Thread] thread
where 1=1
    and thread.IsDeleted = 0
    and thread.VersionOf = thread.Id