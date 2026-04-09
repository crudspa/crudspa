create proc [SamplesCatalog].[BookSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Genres Framework.IdList readonly
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1
declare @genresCount int = (select count(1) from @Genres)

;with BookCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then book.Title
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then book.Title
                end desc,
                case when (@SortField = 'Author' and @SortAscending = 1)
                    then book.Author
                end asc,
                case when (@SortField = 'Author' and @SortAscending = 0)
                    then book.Author
                end desc,
                case when (@SortField = 'Price' and @SortAscending = 1)
                    then book.Price
                end asc,
                case when (@SortField = 'Price' and @SortAscending = 0)
                    then book.Price
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then book.Author
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then book.Author
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then book.Price
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then book.Price
                end desc,
                case when (@SortField = 'Author' and @SortAscending = 1)
                    then book.Title
                end asc,
                case when (@SortField = 'Author' and @SortAscending = 0)
                    then book.Title
                end desc,
                case when (@SortField = 'Author' and @SortAscending = 1)
                    then book.Price
                end asc,
                case when (@SortField = 'Author' and @SortAscending = 0)
                    then book.Price
                end desc,
                case when (@SortField = 'Price' and @SortAscending = 1)
                    then book.Title
                end asc,
                case when (@SortField = 'Price' and @SortAscending = 0)
                    then book.Title
                end desc,
                case when (@SortField = 'Price' and @SortAscending = 1)
                    then book.Author
                end asc,
                case when (@SortField = 'Price' and @SortAscending = 0)
                    then book.Author
                end desc,
                case when (@SortAscending = 1)
                    then book.Id
                end asc,
                case when (@SortAscending = 0)
                    then book.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,book.Id
    from [Samples].[Book-Active] book
        left join [Framework].[ImageFile-Active] coverImage on book.CoverImageId = coverImage.Id
        inner join [Samples].[Genre-Active] genre on book.GenreId = genre.Id
        left join [Framework].[AudioFile-Active] previewAudioFile on book.PreviewAudioFileId = previewAudioFile.Id
        left join [Framework].[PdfFile-Active] samplePdf on book.SamplePdfId = samplePdf.Id
    where 1 = 1

        and (@SearchText is null
            or book.Isbn like '%' + @SearchText + '%'
            or book.Title like '%' + @SearchText + '%'
            or book.Author like '%' + @SearchText + '%'
        )
        and (@genresCount = 0 or book.GenreId in (select Id from @Genres))
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,book.Id
    ,book.Title
    ,coverImage.Id as CoverImageId
    ,coverImage.BlobId as CoverImageBlobId
    ,coverImage.Name as CoverImageName
    ,coverImage.Format as CoverImageFormat
    ,coverImage.Width as CoverImageWidth
    ,coverImage.Height as CoverImageHeight
    ,coverImage.Caption as CoverImageCaption
    ,book.Isbn
    ,book.Author
    ,book.GenreId
    ,genre.Name as GenreName
    ,book.Pages
    ,book.Price
    ,book.Summary
from BookCte cte
    inner join [Samples].[Book-Active] book on cte.Id = book.Id
    left join [Framework].[ImageFile-Active] coverImage on book.CoverImageId = coverImage.Id
    inner join [Samples].[Genre-Active] genre on book.GenreId = genre.Id
    left join [Framework].[AudioFile-Active] previewAudioFile on book.PreviewAudioFileId = previewAudioFile.Id
    left join [Framework].[PdfFile-Active] samplePdf on book.SamplePdfId = samplePdf.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)