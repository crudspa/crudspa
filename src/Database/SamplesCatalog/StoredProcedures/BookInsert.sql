create proc [SamplesCatalog].[BookInsert] (
     @SessionId uniqueidentifier
    ,@Isbn nvarchar(20)
    ,@Title nvarchar(160)
    ,@Author nvarchar(120)
    ,@GenreId uniqueidentifier
    ,@Pages int
    ,@Price real
    ,@Summary nvarchar(max)
    ,@CoverImageId uniqueidentifier
    ,@SamplePdfId uniqueidentifier
    ,@PreviewAudioFileId uniqueidentifier
    ,@Tags Framework.IdList readonly
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Samples].[Book] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Isbn
    ,Title
    ,Author
    ,GenreId
    ,Pages
    ,Price
    ,Summary
    ,CoverImageId
    ,SamplePdfId
    ,PreviewAudioFileId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Isbn
    ,@Title
    ,@Author
    ,@GenreId
    ,@Pages
    ,@Price
    ,@Summary
    ,@CoverImageId
    ,@SamplePdfId
    ,@PreviewAudioFileId
)

insert [Samples].[BookTag] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,BookId
    ,TagId
)
select
     newRow.JunctionId
    ,newRow.JunctionId
    ,@now
    ,@SessionId
    ,@Id
    ,ids.Id
from (select distinct Id from @Tags) ids
    inner join [Samples].[Tag-Active] tag on tag.Id = ids.Id
    cross apply (select newid() as JunctionId) newRow

commit transaction