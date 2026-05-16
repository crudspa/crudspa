create view [Content].[SmsMessageMedia-Active] as

select smsMessageMedia.Id as Id
    ,smsMessageMedia.SmsMessageId as SmsMessageId
    ,smsMessageMedia.ImageId as ImageId
    ,smsMessageMedia.ProviderMediaId as ProviderMediaId
    ,smsMessageMedia.ProviderMediaUrl as ProviderMediaUrl
    ,smsMessageMedia.ContentType as ContentType
    ,smsMessageMedia.FileName as FileName
    ,smsMessageMedia.SizeBytes as SizeBytes
    ,smsMessageMedia.DownloadStatus as DownloadStatus
    ,smsMessageMedia.Downloaded as Downloaded
    ,smsMessageMedia.ErrorMessage as ErrorMessage
    ,smsMessageMedia.Ordinal as Ordinal
from [Content].[SmsMessageMedia] smsMessageMedia
where 1=1