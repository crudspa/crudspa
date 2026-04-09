create view [Education].[GameActivityType-Active] as

select gameActivityType.Id as Id
    ,gameActivityType.Name as Name
    ,gameActivityType.Ordinal as Ordinal
from [Education].[GameActivityType] gameActivityType
where 1=1
    and gameActivityType.IsDeleted = 0