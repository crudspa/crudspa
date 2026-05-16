create proc [FrameworkCore].[SmsWebhookEventStart] (
     @SessionId uniqueidentifier
    ,@SmsChannelKey nvarchar(50)
    ,@Provider int
    ,@Type int
    ,@IdempotencyKey nvarchar(150)
    ,@ProviderMessageId nvarchar(75)
    ,@ProviderStatus nvarchar(75)
    ,@RequestUrl nvarchar(500)
    ,@RequestSignature nvarchar(500)
    ,@SignatureValid bit
    ,@Payload nvarchar(max)
    ,@Status int
    ,@ErrorMessage nvarchar(max)
    ,@Id uniqueidentifier output
    ,@Duplicate bit output
) as

set @Duplicate = 0

select @Id = smsEvent.Id
from [Content].[SmsEvent] smsEvent
where smsEvent.Provider = @Provider
    and smsEvent.IdempotencyKey = @IdempotencyKey

if @Id is not null
begin
    set @Duplicate = 1
    return
end

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Content].[SmsEvent] (
    Id
    ,Updated
    ,UpdatedBy
    ,SmsChannelKey
    ,Provider
    ,Type
    ,IdempotencyKey
    ,ProviderMessageId
    ,ProviderStatus
    ,RequestUrl
    ,RequestSignature
    ,SignatureValid
    ,Received
    ,Processed
    ,Status
    ,Payload
    ,ErrorMessage
)
values (
    @Id
    ,@now
    ,@SessionId
    ,@SmsChannelKey
    ,@Provider
    ,@Type
    ,@IdempotencyKey
    ,@ProviderMessageId
    ,@ProviderStatus
    ,@RequestUrl
    ,@RequestSignature
    ,@SignatureValid
    ,@now
    ,case when @Status = 0 then null else @now end
    ,@Status
    ,@Payload
    ,@ErrorMessage
)