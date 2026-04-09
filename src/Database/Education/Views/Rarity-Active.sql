create view [Education].[Rarity-Active] as

select rarity.Id as Id
    ,rarity.Name as Name
    ,rarity.Ordinal as Ordinal
from [Education].[Rarity] rarity
where 1=1
    and rarity.IsDeleted = 0