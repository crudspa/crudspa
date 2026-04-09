create proc [FrameworkCore].[PdfFileSelectAndLog] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert into [Framework].[PdfViewed] (
    Id
    ,PdfFileId
    ,Viewed
    ,ViewedBy
)
values (
    newid()
    ,@Id
    ,@now
    ,@SessionId
)

select
    pdfFile.Id
    ,pdfFile.BlobId
    ,pdfFile.Name
    ,pdfFile.Format
    ,pdfFile.Description
from [Framework].[PdfFile-Active] pdfFile
where pdfFile.Id = @Id