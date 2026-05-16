create proc [FrameworkCore].[SmsWebhookStatusUpdate] (
     @SessionId uniqueidentifier
    ,@SmsChannelKey nvarchar(50)
    ,@Provider int
    ,@ProviderMessageId nvarchar(75)
    ,@ProviderStatus nvarchar(75)
    ,@Status int
    ,@ProviderErrorCode nvarchar(25)
    ,@ProviderErrorMessage nvarchar(500)
    ,@Id uniqueidentifier output
) as

declare @now datetimeoffset = sysdatetimeoffset()

select @Id = smsMessage.Id
from [Content].[SmsMessage] smsMessage
where smsMessage.Provider = @Provider
    and (smsMessage.SmsChannelKey = @SmsChannelKey or (smsMessage.SmsChannelKey is null and @SmsChannelKey is null))
    and smsMessage.ProviderMessageId = @ProviderMessageId
    and @ProviderMessageId is not null

if @Id is null
    return

update [Content].[SmsMessage]
set Updated = @now
    ,UpdatedBy = @SessionId
    ,Status = @Status
    ,StatusUpdated = @now
    ,ProviderStatus = @ProviderStatus
    ,ProviderErrorCode = @ProviderErrorCode
    ,ProviderErrorMessage = @ProviderErrorMessage
where Id = @Id