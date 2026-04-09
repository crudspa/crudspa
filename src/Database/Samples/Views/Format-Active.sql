create view [Samples].[Format-Active] as

select format.Id as Id
    ,format.Name as Name
    ,format.Ordinal as Ordinal
from [Samples].[Format] format
where 1=1
    and format.IsDeleted = 0