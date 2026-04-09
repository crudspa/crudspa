create view [Framework].[PdfFile-Active] as

select pdfFile.Id as Id
    ,pdfFile.BlobId as BlobId
    ,pdfFile.Name as Name
    ,pdfFile.Format as Format
    ,pdfFile.Description as Description
from [Framework].[PdfFile] pdfFile
where 1=1
    and pdfFile.IsDeleted = 0