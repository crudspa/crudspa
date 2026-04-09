create view [Content].[ForumBundle-Active] as

select forumBundle.Id as Id
    ,forumBundle.ForumId as ForumId
    ,forumBundle.BundleId as BundleId
    ,forumBundle.ThreadRule as ThreadRule
    ,forumBundle.CommentRule as CommentRule
from [Content].[ForumBundle] forumBundle
where 1=1
    and forumBundle.IsDeleted = 0
    and forumBundle.VersionOf = forumBundle.Id