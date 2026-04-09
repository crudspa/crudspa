create view [Content].[AlignItems-Active] as

select alignItems.Id as Id
    ,alignItems.Name as Name
    ,alignItems.Ordinal as Ordinal
from [Content].[AlignItems] alignItems
where 1=1
    and alignItems.IsDeleted = 0