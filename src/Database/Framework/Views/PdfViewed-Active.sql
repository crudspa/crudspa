create view [Framework].[PdfViewed-Active] as

select pdfViewed.Id as Id
    ,pdfViewed.PdfFileId as PdfFileId
    ,pdfViewed.Viewed as Viewed
    ,pdfViewed.ViewedBy as ViewedBy
from [Framework].[PdfViewed] pdfViewed
where 1=1