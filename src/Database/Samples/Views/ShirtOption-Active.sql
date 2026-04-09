create view [Samples].[ShirtOption-Active] as

select shirtOption.Id as Id
    ,shirtOption.ShirtId as ShirtId
    ,shirtOption.ColorId as ColorId
    ,shirtOption.SkuBase as SkuBase
    ,shirtOption.Price as Price
    ,shirtOption.AllSizes as AllSizes
    ,shirtOption.Ordinal as Ordinal
from [Samples].[ShirtOption] shirtOption
where 1=1
    and shirtOption.IsDeleted = 0
    and shirtOption.VersionOf = shirtOption.Id