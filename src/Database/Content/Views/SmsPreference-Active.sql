create view [Content].[SmsPreference-Active] as

select smsPreference.Id as Id
    ,smsPreference.PortalId as PortalId
    ,smsPreference.OrganizationId as OrganizationId
    ,smsPreference.SmsChannelKey as SmsChannelKey
    ,smsPreference.ContactId as ContactId
    ,smsPreference.ContactPhoneId as ContactPhoneId
    ,smsPreference.Number as Number
    ,smsPreference.Status as Status
    ,smsPreference.Source as Source
    ,smsPreference.StatusChanged as StatusChanged
    ,smsPreference.SmsMessageId as SmsMessageId
    ,smsPreference.Notes as Notes
from [Content].[SmsPreference] smsPreference
where 1=1
    and smsPreference.IsDeleted = 0
    and smsPreference.VersionOf = smsPreference.Id