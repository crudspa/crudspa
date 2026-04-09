create trigger [Samples].[ShirtTrigger] on [Samples].[Shirt]
    for update
as

insert [Samples].[Shirt] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Name
    ,BrandId
    ,Fit
    ,Material
    ,Price
    ,HeroImageId
    ,GuidePdfId
    ,Featured
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Name
    ,deleted.BrandId
    ,deleted.Fit
    ,deleted.Material
    ,deleted.Price
    ,deleted.HeroImageId
    ,deleted.GuidePdfId
    ,deleted.Featured
from deleted