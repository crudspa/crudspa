create proc [SamplesCatalog].[ShirtSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     shirt.Id
    ,shirt.Name
    ,shirt.BrandId
    ,brand.Name as BrandName
    ,shirt.Fit
    ,shirt.Material
    ,shirt.Price
    ,heroImage.Id as HeroImageId
    ,heroImage.BlobId as HeroImageBlobId
    ,heroImage.Name as HeroImageName
    ,heroImage.Format as HeroImageFormat
    ,heroImage.Width as HeroImageWidth
    ,heroImage.Height as HeroImageHeight
    ,heroImage.Caption as HeroImageCaption
    ,guidePdf.Id as GuidePdfId
    ,guidePdf.BlobId as GuidePdfBlobId
    ,guidePdf.Name as GuidePdfName
    ,guidePdf.Format as GuidePdfFormat
    ,guidePdf.Description as GuidePdfDescription
    ,(select count(1) from [Samples].[ShirtOption-Active] where ShirtId = shirt.Id) as ShirtOptionCount
from [Samples].[Shirt-Active] shirt
    inner join [Samples].[Brand-Active] brand on shirt.BrandId = brand.Id
    left join [Framework].[PdfFile-Active] guidePdf on shirt.GuidePdfId = guidePdf.Id
    left join [Framework].[ImageFile-Active] heroImage on shirt.HeroImageId = heroImage.Id
where shirt.Id = @Id