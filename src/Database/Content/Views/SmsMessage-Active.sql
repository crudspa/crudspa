create view [Content].[SmsMessage-Active] as

select smsMessage.Id as Id
    ,smsMessage.SmsId as SmsId
    ,smsMessage.SmsChannelKey as SmsChannelKey
    ,smsMessage.MembershipId as MembershipId
    ,smsMessage.MemberId as MemberId
    ,smsMessage.ContactId as ContactId
    ,smsMessage.ContactPhoneId as ContactPhoneId
    ,smsMessage.Direction as Direction
    ,smsMessage.Body as Body
    ,smsMessage.FromNumber as FromNumber
    ,smsMessage.ToNumber as ToNumber
    ,smsMessage.Occurred as Occurred
    ,smsMessage.Status as Status
    ,smsMessage.StatusUpdated as StatusUpdated
    ,smsMessage.Provider as Provider
    ,smsMessage.ProviderMessageId as ProviderMessageId
    ,smsMessage.ProviderStatus as ProviderStatus
    ,smsMessage.ProviderErrorCode as ProviderErrorCode
    ,smsMessage.ProviderErrorMessage as ProviderErrorMessage
    ,smsMessage.SegmentCount as SegmentCount
    ,smsMessage.ApiResponse as ApiResponse
from [Content].[SmsMessage] smsMessage
where 1=1