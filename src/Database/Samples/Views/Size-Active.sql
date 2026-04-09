create view [Samples].[Size-Active] as

select sizeTable.Id as Id
    ,sizeTable.ColorId as ColorId
    ,sizeTable.Name as Name
    ,sizeTable.Ordinal as Ordinal
from [Samples].[Size] sizeTable
where 1=1
    and sizeTable.IsDeleted = 0