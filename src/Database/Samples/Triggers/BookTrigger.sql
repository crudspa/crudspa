create trigger [Samples].[BookTrigger] on [Samples].[Book]
    for update
as

insert [Samples].[Book] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ExternalId
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
    ,Featured
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ExternalId
    ,deleted.Isbn
    ,deleted.Title
    ,deleted.Author
    ,deleted.GenreId
    ,deleted.Pages
    ,deleted.Price
    ,deleted.Summary
    ,deleted.CoverImageId
    ,deleted.SamplePdfId
    ,deleted.PreviewAudioFileId
    ,deleted.Featured
from deleted