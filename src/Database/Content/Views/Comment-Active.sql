create view [Content].[Comment-Active] as

select comment.Id as Id
    ,comment.ParentId as ParentId
    ,comment.PostId as PostId
    ,comment.ThreadId as ThreadId
    ,comment.ById as ById
    ,comment.Posted as Posted
    ,comment.Edited as Edited
    ,comment.Body as Body
from [Content].[Comment] comment
where 1=1
    and comment.IsDeleted = 0
    and comment.VersionOf = comment.Id