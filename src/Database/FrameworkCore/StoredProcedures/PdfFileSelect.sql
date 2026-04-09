create proc [FrameworkCore].[PdfFileSelect] (
     @Id uniqueidentifier
) as

select
    pdfFile.Id
    ,pdfFile.BlobId
    ,pdfFile.Name
    ,pdfFile.Format
    ,pdfFile.Description
from [Framework].[PdfFile-Active] pdfFile
where pdfFile.Id = @Id