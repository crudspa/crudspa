create view [Samples].[ShirtOptionSize-Active] as

select shirtOptionSize.Id as Id
    ,shirtOptionSize.ShirtOptionId as ShirtOptionId
    ,shirtOptionSize.SizeId as SizeId
from [Samples].[ShirtOptionSize] shirtOptionSize
where 1=1
    and shirtOptionSize.IsDeleted = 0
    and shirtOptionSize.VersionOf = shirtOptionSize.Id