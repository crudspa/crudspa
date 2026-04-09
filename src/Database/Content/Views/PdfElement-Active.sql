create view [Content].[PdfElement-Active] as

select pdfElement.Id as Id
    ,pdfElement.ElementId as ElementId
    ,pdfElement.FileId as FileId
from [Content].[PdfElement] pdfElement
where 1=1
    and pdfElement.IsDeleted = 0
    and pdfElement.VersionOf = pdfElement.Id