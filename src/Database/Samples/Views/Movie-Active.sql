create view [Samples].[Movie-Active] as

select movie.Id as Id
    ,movie.ExternalId as ExternalId
    ,movie.Title as Title
    ,movie.GenreId as GenreId
    ,movie.RatingId as RatingId
    ,movie.Released as Released
    ,movie.RuntimeMin as RuntimeMin
    ,movie.Score as Score
    ,movie.Summary as Summary
    ,movie.PosterImageId as PosterImageId
    ,movie.TrailerVideoId as TrailerVideoId
    ,movie.Featured as Featured
from [Samples].[Movie] movie
where 1=1
    and movie.IsDeleted = 0
    and movie.VersionOf = movie.Id