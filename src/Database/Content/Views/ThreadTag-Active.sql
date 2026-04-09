create view [Content].[ThreadTag-Active] as

select threadTag.Id as Id
    ,threadTag.ThreadId as ThreadId
    ,threadTag.TagId as TagId
from [Content].[ThreadTag] threadTag
where 1=1
    and threadTag.IsDeleted = 0
    and threadTag.VersionOf = threadTag.Id