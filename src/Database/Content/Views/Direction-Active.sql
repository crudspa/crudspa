create view [Content].[Direction-Active] as

select direction.Id as Id
    ,direction.Name as Name
    ,direction.Ordinal as Ordinal
from [Content].[Direction] direction
where 1=1
    and direction.IsDeleted = 0