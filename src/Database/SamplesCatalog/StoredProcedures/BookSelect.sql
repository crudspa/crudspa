create proc [SamplesCatalog].[BookSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     book.Id
    ,book.Title
    ,book.Isbn
    ,book.Author
    ,book.GenreId
    ,genre.Name as GenreName
    ,book.Pages
    ,book.Price
    ,book.Summary
    ,coverImage.Id as CoverImageId
    ,coverImage.BlobId as CoverImageBlobId
    ,coverImage.Name as CoverImageName
    ,coverImage.Format as CoverImageFormat
    ,coverImage.Width as CoverImageWidth
    ,coverImage.Height as CoverImageHeight
    ,coverImage.Caption as CoverImageCaption
    ,samplePdf.Id as SamplePdfId
    ,samplePdf.BlobId as SamplePdfBlobId
    ,samplePdf.Name as SamplePdfName
    ,samplePdf.Format as SamplePdfFormat
    ,samplePdf.Description as SamplePdfDescription
    ,previewAudioFile.Id as PreviewAudioFileId
    ,previewAudioFile.BlobId as PreviewAudioFileBlobId
    ,previewAudioFile.Name as PreviewAudioFileName
    ,previewAudioFile.Format as PreviewAudioFileFormat
    ,previewAudioFile.OptimizedStatus as PreviewAudioFileOptimizedStatus
    ,previewAudioFile.OptimizedBlobId as PreviewAudioFileOptimizedBlobId
    ,previewAudioFile.OptimizedFormat as PreviewAudioFileOptimizedFormat
from [Samples].[Book-Active] book
    left join [Framework].[ImageFile-Active] coverImage on book.CoverImageId = coverImage.Id
    inner join [Samples].[Genre-Active] genre on book.GenreId = genre.Id
    left join [Framework].[AudioFile-Active] previewAudioFile on book.PreviewAudioFileId = previewAudioFile.Id
    left join [Framework].[PdfFile-Active] samplePdf on book.SamplePdfId = samplePdf.Id
where book.Id = @Id


select
     bookEdition.Id
    ,bookEdition.BookId
    ,book.Title as BookTitle
    ,bookEdition.FormatId
    ,format.Name as FormatName
    ,bookEdition.Sku
    ,bookEdition.Price
    ,bookEdition.ReleasedOn
    ,bookEdition.InPrint
    ,bookEdition.Ordinal
from [Samples].[BookEdition-Active] bookEdition
    inner join [Samples].[Book-Active] book on bookEdition.BookId = book.Id
    inner join [Samples].[Format-Active] format on bookEdition.FormatId = format.Id
where bookEdition.BookId = @Id

select distinct
     @Id as BookId
    ,tag.Id as TagId
    ,tag.Name as TagName
    ,convert(bit, iif(bookTag.Id is null, 0, 1)) as Selected
    ,tag.Ordinal
from [Samples].[Tag-Active] tag
    left join [Samples].[BookTag-Active] bookTag on bookTag.TagId = tag.Id
        and bookTag.BookId = @Id
order by tag.Ordinal