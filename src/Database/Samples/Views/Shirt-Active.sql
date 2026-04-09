create view [Samples].[Shirt-Active] as

select shirt.Id as Id
    ,shirt.Name as Name
    ,shirt.BrandId as BrandId
    ,shirt.Fit as Fit
    ,shirt.Material as Material
    ,shirt.Price as Price
    ,shirt.HeroImageId as HeroImageId
    ,shirt.GuidePdfId as GuidePdfId
    ,shirt.Featured as Featured
from [Samples].[Shirt] shirt
where 1=1
    and shirt.IsDeleted = 0
    and shirt.VersionOf = shirt.Id