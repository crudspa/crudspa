create proc [ContentJobs].[SmsMessageMediaInsert] (
     @SessionId uniqueidentifier
    ,@SmsMessageId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@ProviderMediaUrl nvarchar(500)
    ,@ContentType nvarchar(100)
    ,@FileName nvarchar(150)
    ,@SizeBytes int
    ,@DownloadStatus int
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert [Content].[SmsMessageMedia] (
     Id
    ,Updated
    ,UpdatedBy
    ,SmsMessageId
    ,ImageId
    ,ProviderMediaUrl
    ,ContentType
    ,FileName
    ,SizeBytes
    ,DownloadStatus
    ,Downloaded
    ,Ordinal
) values (
     newid()
    ,@now
    ,@SessionId
    ,@SmsMessageId
    ,@ImageId
    ,@ProviderMediaUrl
    ,@ContentType
    ,@FileName
    ,@SizeBytes
    ,@DownloadStatus
    ,case when @DownloadStatus = 1 then @now else null end
    ,@Ordinal
)