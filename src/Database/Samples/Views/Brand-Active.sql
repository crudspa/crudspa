create view [Samples].[Brand-Active] as

select brand.Id as Id
    ,brand.Name as Name
    ,brand.Ordinal as Ordinal
from [Samples].[Brand] brand
where 1=1
    and brand.IsDeleted = 0