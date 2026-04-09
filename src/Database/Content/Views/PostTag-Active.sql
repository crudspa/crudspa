create view [Content].[PostTag-Active] as

select postTag.Id as Id
    ,postTag.PostId as PostId
    ,postTag.TagId as TagId
from [Content].[PostTag] postTag
where 1=1
    and postTag.IsDeleted = 0
    and postTag.VersionOf = postTag.Id