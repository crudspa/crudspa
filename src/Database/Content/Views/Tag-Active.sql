create view [Content].[Tag-Active] as

select tag.Id as Id
    ,tag.Title as Title
from [Content].[Tag] tag
where 1=1
    and tag.IsDeleted = 0
    and tag.VersionOf = tag.Id