create view [Education].[ContentCategory-Active] as

select contentCategory.Id as Id
    ,contentCategory.Name as Name
    ,contentCategory.Ordinal as Ordinal
from [Education].[ContentCategory] contentCategory
where 1=1
    and contentCategory.IsDeleted = 0