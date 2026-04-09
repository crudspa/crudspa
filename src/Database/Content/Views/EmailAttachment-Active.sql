create view [Content].[EmailAttachment-Active] as

select emailAttachment.Id as Id
    ,emailAttachment.EmailId as EmailId
    ,emailAttachment.PdfId as PdfId
    ,emailAttachment.Ordinal as Ordinal
from [Content].[EmailAttachment] emailAttachment
where 1=1
    and emailAttachment.IsDeleted = 0