create proc [SamplesCatalog].[MovieSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     movie.Id
    ,movie.Title
    ,movie.GenreId
    ,genre.Name as GenreName
    ,movie.RatingId
    ,rating.Name as RatingName
    ,movie.Released
    ,movie.RuntimeMin
    ,movie.Score
    ,movie.Summary
    ,posterImage.Id as PosterImageId
    ,posterImage.BlobId as PosterImageBlobId
    ,posterImage.Name as PosterImageName
    ,posterImage.Format as PosterImageFormat
    ,posterImage.Width as PosterImageWidth
    ,posterImage.Height as PosterImageHeight
    ,posterImage.Caption as PosterImageCaption
    ,trailerVideo.Id as TrailerVideoId
    ,trailerVideo.BlobId as TrailerVideoBlobId
    ,trailerVideo.Name as TrailerVideoName
    ,trailerVideo.Format as TrailerVideoFormat
    ,trailerVideo.Width as TrailerVideoWidth
    ,trailerVideo.Height as TrailerVideoHeight
    ,trailerVideo.OptimizedStatus as TrailerVideoOptimizedStatus
    ,trailerVideo.OptimizedBlobId as TrailerVideoOptimizedBlobId
    ,trailerVideo.OptimizedFormat as TrailerVideoOptimizedFormat
    ,(select count(1) from [Samples].[MovieCredit-Active] where MovieId = movie.Id) as MovieCreditCount
from [Samples].[Movie-Active] movie
    inner join [Samples].[Genre-Active] genre on movie.GenreId = genre.Id
    left join [Framework].[ImageFile-Active] posterImage on movie.PosterImageId = posterImage.Id
    inner join [Samples].[Rating-Active] rating on movie.RatingId = rating.Id
    left join [Framework].[VideoFile-Active] trailerVideo on movie.TrailerVideoId = trailerVideo.Id
where movie.Id = @Id