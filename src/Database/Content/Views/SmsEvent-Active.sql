create view [Content].[SmsEvent-Active] as

select smsEvent.Id as Id
    ,smsEvent.SmsMessageId as SmsMessageId
    ,smsEvent.SmsChannelKey as SmsChannelKey
    ,smsEvent.Provider as Provider
    ,smsEvent.Type as Type
    ,smsEvent.IdempotencyKey as IdempotencyKey
    ,smsEvent.ProviderMessageId as ProviderMessageId
    ,smsEvent.ProviderStatus as ProviderStatus
    ,smsEvent.RequestUrl as RequestUrl
    ,smsEvent.RequestSignature as RequestSignature
    ,smsEvent.SignatureValid as SignatureValid
    ,smsEvent.Received as Received
    ,smsEvent.Processed as Processed
    ,smsEvent.Status as Status
    ,smsEvent.Payload as Payload
    ,smsEvent.ErrorMessage as ErrorMessage
from [Content].[SmsEvent] smsEvent
where 1=1