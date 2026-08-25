create proc [ContentMessaging].[SmsMessageMediaSelectForSmsMessage] (
     @SessionId uniqueidentifier
    ,@SmsMessageId uniqueidentifier
) as

set nocount on

select
     smsMessageMedia.Id
    ,smsMessageMedia.SmsMessageId
    ,smsMessageMedia.FileName
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,smsMessageMedia.ContentType
    ,smsMessageMedia.SizeBytes
    ,smsMessageMedia.DownloadStatus
    ,smsMessageMedia.Ordinal
from [Content].[SmsMessageMedia-Active] smsMessageMedia
    left join [Framework].[ImageFile-Active] image on smsMessageMedia.ImageId = image.Id
    inner join [Content].[SmsMessage-Active] smsMessage on smsMessageMedia.SmsMessageId = smsMessage.Id
    inner join [Content].[Membership-Active] membership on smsMessage.MembershipId = membership.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
where smsMessageMedia.SmsMessageId = @SmsMessageId