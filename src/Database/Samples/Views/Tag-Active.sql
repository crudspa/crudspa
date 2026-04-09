create view [Samples].[Tag-Active] as

select tag.Id as Id
    ,tag.Name as Name
    ,tag.Ordinal as Ordinal
from [Samples].[Tag] tag
where 1=1
    and tag.IsDeleted = 0