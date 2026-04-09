create proc [SamplesCatalog].[MovieSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Genres Framework.IdList readonly
    ,@Ratings Framework.IdList readonly
    ,@ReleasedStart datetimeoffset(7)
    ,@ReleasedEnd datetimeoffset(7)
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1
declare @genresCount int = (select count(1) from @Genres)
declare @ratingsCount int = (select count(1) from @Ratings)

;with MovieCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then movie.Title
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then movie.Title
                end desc,
                case when (@SortField = 'Released' and @SortAscending = 1)
                    then movie.Released
                end asc,
                case when (@SortField = 'Released' and @SortAscending = 0)
                    then movie.Released
                end desc,
                case when (@SortField = 'Score' and @SortAscending = 1)
                    then movie.Score
                end asc,
                case when (@SortField = 'Score' and @SortAscending = 0)
                    then movie.Score
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then movie.Released
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then movie.Released
                end desc,
                case when (@SortField = 'Title' and @SortAscending = 1)
                    then movie.Score
                end asc,
                case when (@SortField = 'Title' and @SortAscending = 0)
                    then movie.Score
                end desc,
                case when (@SortField = 'Released' and @SortAscending = 1)
                    then movie.Title
                end asc,
                case when (@SortField = 'Released' and @SortAscending = 0)
                    then movie.Title
                end desc,
                case when (@SortField = 'Released' and @SortAscending = 1)
                    then movie.Score
                end asc,
                case when (@SortField = 'Released' and @SortAscending = 0)
                    then movie.Score
                end desc,
                case when (@SortField = 'Score' and @SortAscending = 1)
                    then movie.Title
                end asc,
                case when (@SortField = 'Score' and @SortAscending = 0)
                    then movie.Title
                end desc,
                case when (@SortField = 'Score' and @SortAscending = 1)
                    then movie.Released
                end asc,
                case when (@SortField = 'Score' and @SortAscending = 0)
                    then movie.Released
                end desc,
                case when (@SortAscending = 1)
                    then movie.Id
                end asc,
                case when (@SortAscending = 0)
                    then movie.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,movie.Id
    from [Samples].[Movie-Active] movie
        inner join [Samples].[Genre-Active] genre on movie.GenreId = genre.Id
        left join [Framework].[ImageFile-Active] posterImage on movie.PosterImageId = posterImage.Id
        inner join [Samples].[Rating-Active] rating on movie.RatingId = rating.Id
        left join [Framework].[VideoFile-Active] trailerVideo on movie.TrailerVideoId = trailerVideo.Id
    where 1 = 1

        and (@SearchText is null
            or movie.Title like '%' + @SearchText + '%'
        )
        and (@genresCount = 0 or movie.GenreId in (select Id from @Genres))
        and (@ratingsCount = 0 or movie.RatingId in (select Id from @Ratings))
        and (@ReleasedStart is null or movie.Released >= @ReleasedStart)
        and (@ReleasedEnd is null or movie.Released < @ReleasedEnd)
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,movie.Id
    ,movie.Title
    ,posterImage.Id as PosterImageId
    ,posterImage.BlobId as PosterImageBlobId
    ,posterImage.Name as PosterImageName
    ,posterImage.Format as PosterImageFormat
    ,posterImage.Width as PosterImageWidth
    ,posterImage.Height as PosterImageHeight
    ,posterImage.Caption as PosterImageCaption
    ,movie.GenreId
    ,genre.Name as GenreName
    ,movie.RatingId
    ,rating.Name as RatingName
    ,movie.Released
    ,movie.RuntimeMin
    ,movie.Score
    ,movie.Summary
    ,(select count(1) from [Samples].[MovieCredit-Active] where MovieId = movie.Id) as MovieCreditCount
from MovieCte cte
    inner join [Samples].[Movie-Active] movie on cte.Id = movie.Id
    inner join [Samples].[Genre-Active] genre on movie.GenreId = genre.Id
    left join [Framework].[ImageFile-Active] posterImage on movie.PosterImageId = posterImage.Id
    inner join [Samples].[Rating-Active] rating on movie.RatingId = rating.Id
    left join [Framework].[VideoFile-Active] trailerVideo on movie.TrailerVideoId = trailerVideo.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)