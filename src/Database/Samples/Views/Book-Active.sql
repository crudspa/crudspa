create view [Samples].[Book-Active] as

select book.Id as Id
    ,book.ExternalId as ExternalId
    ,book.Isbn as Isbn
    ,book.Title as Title
    ,book.Author as Author
    ,book.GenreId as GenreId
    ,book.Pages as Pages
    ,book.Price as Price
    ,book.Summary as Summary
    ,book.CoverImageId as CoverImageId
    ,book.SamplePdfId as SamplePdfId
    ,book.PreviewAudioFileId as PreviewAudioFileId
    ,book.Featured as Featured
from [Samples].[Book] book
where 1=1
    and book.IsDeleted = 0
    and book.VersionOf = book.Id