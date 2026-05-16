create proc [FrameworkCore].[SmsWebhookMediaUpsert] (
     @SessionId uniqueidentifier
    ,@SmsMessageId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@ProviderMediaId nvarchar(75)
    ,@ProviderMediaUrl nvarchar(500)
    ,@ContentType nvarchar(100)
    ,@FileName nvarchar(150)
    ,@SizeBytes int
    ,@DownloadStatus int
    ,@ErrorMessage nvarchar(500)
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

declare @now datetimeoffset = sysdatetimeoffset()

select @Id = media.Id
from [Content].[SmsMessageMedia] media
where media.SmsMessageId = @SmsMessageId
    and media.Ordinal = @Ordinal

if @Id is not null
begin
    update [Content].[SmsMessageMedia]
    set Updated = @now
        ,UpdatedBy = @SessionId
        ,ImageId = @ImageId
        ,ProviderMediaId = @ProviderMediaId
        ,ProviderMediaUrl = @ProviderMediaUrl
        ,ContentType = @ContentType
        ,FileName = @FileName
        ,SizeBytes = @SizeBytes
        ,DownloadStatus = @DownloadStatus
        ,Downloaded = case when @DownloadStatus = 1 then @now else null end
        ,ErrorMessage = @ErrorMessage
    where Id = @Id

    return
end

set @Id = newid()

insert [Content].[SmsMessageMedia] (
    Id
    ,Updated
    ,UpdatedBy
    ,SmsMessageId
    ,ImageId
    ,ProviderMediaId
    ,ProviderMediaUrl
    ,ContentType
    ,FileName
    ,SizeBytes
    ,DownloadStatus
    ,Downloaded
    ,ErrorMessage
    ,Ordinal
)
values (
    @Id
    ,@now
    ,@SessionId
    ,@SmsMessageId
    ,@ImageId
    ,@ProviderMediaId
    ,@ProviderMediaUrl
    ,@ContentType
    ,@FileName
    ,@SizeBytes
    ,@DownloadStatus
    ,case when @DownloadStatus = 1 then @now else null end
    ,@ErrorMessage
    ,@Ordinal
)