create proc [SamplesCatalog].[MovieUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(160)
    ,@GenreId uniqueidentifier
    ,@RatingId uniqueidentifier
    ,@Released datetimeoffset(7)
    ,@RuntimeMin int
    ,@Score real
    ,@Summary nvarchar(max)
    ,@PosterImageId uniqueidentifier
    ,@TrailerVideoId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,GenreId = @GenreId
    ,RatingId = @RatingId
    ,Released = @Released
    ,RuntimeMin = @RuntimeMin
    ,Score = @Score
    ,Summary = @Summary
    ,PosterImageId = @PosterImageId
    ,TrailerVideoId = @TrailerVideoId
from [Samples].[Movie] baseTable
    inner join [Samples].[Movie-Active] movie on movie.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction