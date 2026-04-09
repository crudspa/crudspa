create proc [SamplesCatalog].[ShirtSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Brands Framework.IdList readonly
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1
declare @brandsCount int = (select count(1) from @Brands)

;with ShirtCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Name' and @SortAscending = 1)
                    then shirt.Name
                end asc,
                case when (@SortField = 'Name' and @SortAscending = 0)
                    then shirt.Name
                end desc,
                case when (@SortField = 'Price' and @SortAscending = 1)
                    then shirt.Price
                end asc,
                case when (@SortField = 'Price' and @SortAscending = 0)
                    then shirt.Price
                end desc,
                case when (@SortField = 'Name' and @SortAscending = 1)
                    then shirt.Price
                end asc,
                case when (@SortField = 'Name' and @SortAscending = 0)
                    then shirt.Price
                end desc,
                case when (@SortField = 'Price' and @SortAscending = 1)
                    then shirt.Name
                end asc,
                case when (@SortField = 'Price' and @SortAscending = 0)
                    then shirt.Name
                end desc,
                case when (@SortAscending = 1)
                    then shirt.Id
                end asc,
                case when (@SortAscending = 0)
                    then shirt.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,shirt.Id
    from [Samples].[Shirt-Active] shirt
        inner join [Samples].[Brand-Active] brand on shirt.BrandId = brand.Id
        left join [Framework].[PdfFile-Active] guidePdf on shirt.GuidePdfId = guidePdf.Id
        left join [Framework].[ImageFile-Active] heroImage on shirt.HeroImageId = heroImage.Id
    where 1 = 1

        and (@SearchText is null
            or shirt.Name like '%' + @SearchText + '%'
        )
        and (@brandsCount = 0 or shirt.BrandId in (select Id from @Brands))
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,shirt.Id
    ,shirt.Name
    ,heroImage.Id as HeroImageId
    ,heroImage.BlobId as HeroImageBlobId
    ,heroImage.Name as HeroImageName
    ,heroImage.Format as HeroImageFormat
    ,heroImage.Width as HeroImageWidth
    ,heroImage.Height as HeroImageHeight
    ,heroImage.Caption as HeroImageCaption
    ,shirt.BrandId
    ,brand.Name as BrandName
    ,shirt.Fit
    ,shirt.Material
    ,shirt.Price
    ,(select count(1) from [Samples].[ShirtOption-Active] where ShirtId = shirt.Id) as ShirtOptionCount
from ShirtCte cte
    inner join [Samples].[Shirt-Active] shirt on cte.Id = shirt.Id
    inner join [Samples].[Brand-Active] brand on shirt.BrandId = brand.Id
    left join [Framework].[PdfFile-Active] guidePdf on shirt.GuidePdfId = guidePdf.Id
    left join [Framework].[ImageFile-Active] heroImage on shirt.HeroImageId = heroImage.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)