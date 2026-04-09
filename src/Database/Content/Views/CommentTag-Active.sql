create view [Content].[CommentTag-Active] as

select commentTag.Id as Id
    ,commentTag.CommentId as CommentId
    ,commentTag.TagId as TagId
from [Content].[CommentTag] commentTag
where 1=1
    and commentTag.IsDeleted = 0
    and commentTag.VersionOf = commentTag.Id