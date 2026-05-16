create view [Content].[SmsAttachment-Active] as

select smsAttachment.Id as Id
    ,smsAttachment.SmsId as SmsId
    ,smsAttachment.ImageId as ImageId
    ,smsAttachment.Ordinal as Ordinal
from [Content].[SmsAttachment] smsAttachment
where 1=1
    and smsAttachment.IsDeleted = 0