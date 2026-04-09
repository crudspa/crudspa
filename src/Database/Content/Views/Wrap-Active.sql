create view [Content].[Wrap-Active] as

select wrap.Id as Id
    ,wrap.Name as Name
    ,wrap.Ordinal as Ordinal
from [Content].[Wrap] wrap
where 1=1
    and wrap.IsDeleted = 0