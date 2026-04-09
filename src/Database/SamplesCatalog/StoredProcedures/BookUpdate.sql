create proc [SamplesCatalog].[BookUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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
    ,Isbn = @Isbn
    ,Title = @Title
    ,Author = @Author
    ,GenreId = @GenreId
    ,Pages = @Pages
    ,Price = @Price
    ,Summary = @Summary
    ,CoverImageId = @CoverImageId
    ,SamplePdfId = @SamplePdfId
    ,PreviewAudioFileId = @PreviewAudioFileId
from [Samples].[Book] baseTable
    inner join [Samples].[Book-Active] book on book.Id = baseTable.Id
where baseTable.Id = @Id


update bookTag
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Samples].[BookTag] bookTag
    left join @Tags ids on ids.Id = bookTag.TagId
where bookTag.BookId = @Id
    and bookTag.IsDeleted = 0
    and ids.Id is null

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
    left join [Samples].[BookTag-Active] existingJunction on existingJunction.BookId = @Id
        and existingJunction.TagId = ids.Id
    cross apply (select newid() as JunctionId) newRow
where existingJunction.Id is null
commit transaction