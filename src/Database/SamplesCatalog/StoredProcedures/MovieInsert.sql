create proc [SamplesCatalog].[MovieInsert] (
     @SessionId uniqueidentifier
    ,@Title nvarchar(160)
    ,@GenreId uniqueidentifier
    ,@RatingId uniqueidentifier
    ,@Released datetimeoffset(7)
    ,@RuntimeMin int
    ,@Score real
    ,@Summary nvarchar(max)
    ,@PosterImageId uniqueidentifier
    ,@TrailerVideoId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Samples].[Movie] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Title
    ,GenreId
    ,RatingId
    ,Released
    ,RuntimeMin
    ,Score
    ,Summary
    ,PosterImageId
    ,TrailerVideoId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Title
    ,@GenreId
    ,@RatingId
    ,@Released
    ,@RuntimeMin
    ,@Score
    ,@Summary
    ,@PosterImageId
    ,@TrailerVideoId
)

commit transaction