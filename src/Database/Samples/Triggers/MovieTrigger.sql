create trigger [Samples].[MovieTrigger] on [Samples].[Movie]
    for update
as

insert [Samples].[Movie] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ExternalId
    ,Title
    ,GenreId
    ,RatingId
    ,Released
    ,RuntimeMin
    ,Score
    ,Summary
    ,PosterImageId
    ,TrailerVideoId
    ,Featured
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ExternalId
    ,deleted.Title
    ,deleted.GenreId
    ,deleted.RatingId
    ,deleted.Released
    ,deleted.RuntimeMin
    ,deleted.Score
    ,deleted.Summary
    ,deleted.PosterImageId
    ,deleted.TrailerVideoId
    ,deleted.Featured
from deleted