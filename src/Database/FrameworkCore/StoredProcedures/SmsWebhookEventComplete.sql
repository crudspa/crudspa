create proc [FrameworkCore].[SmsWebhookEventComplete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@SmsMessageId uniqueidentifier
    ,@Status int
    ,@ErrorMessage nvarchar(max)
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Content].[SmsEvent]
set Updated = @now
    ,UpdatedBy = @SessionId
    ,SmsMessageId = @SmsMessageId
    ,Processed = @now
    ,Status = @Status
    ,ErrorMessage = @ErrorMessage
where Id = @Id