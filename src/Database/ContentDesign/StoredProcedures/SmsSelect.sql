create proc [ContentDesign].[SmsSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     sms.Id
    ,sms.MembershipId
    ,sms.Body
    ,sms.TemplateId
    ,template.Title as TemplateTitle
    ,sms.Send
    ,sms.Status
    ,sms.Processed
from [Content].[Sms-Active] sms
    inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
    left join [Content].[SmsTemplate-Active] template on sms.TemplateId = template.Id
where sms.Id = @Id


select
     smsAttachment.Id
    ,smsAttachment.SmsId
    ,sms.Body as SmsBody
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,smsAttachment.Ordinal
from [Content].[SmsAttachment-Active] smsAttachment
    inner join [Framework].[ImageFile-Active] image on smsAttachment.ImageId = image.Id
    inner join [Content].[Sms-Active] sms on smsAttachment.SmsId = sms.Id
where smsAttachment.SmsId = @Id