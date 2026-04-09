create view [Samples].[Color-Active] as

select color.Id as Id
    ,color.Name as Name
    ,color.Ordinal as Ordinal
from [Samples].[Color] color
where 1=1
    and color.IsDeleted = 0