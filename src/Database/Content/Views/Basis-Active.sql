create view [Content].[Basis-Active] as

select basis.Id as Id
    ,basis.Name as Name
    ,basis.Ordinal as Ordinal
from [Content].[Basis] basis
where 1=1
    and basis.IsDeleted = 0