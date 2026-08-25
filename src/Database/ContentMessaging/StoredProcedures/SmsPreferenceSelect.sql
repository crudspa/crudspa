create proc [ContentMessaging].[SmsPreferenceSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     smsPreference.Id
    ,smsPreference.PortalId
    ,smsPreference.OrganizationId
    ,smsPreference.Number
    ,smsPreference.ContactId
    ,contact.FirstName as ContactFirstName
    ,smsPreference.ContactPhoneId
    ,contactPhone.Phone as ContactPhonePhone
    ,smsPreference.Status
    ,smsPreference.Source
    ,smsPreference.StatusChanged
    ,smsPreference.Notes
from [Content].[SmsPreference-Active] smsPreference
    left join [Framework].[Contact-Active] contact on smsPreference.ContactId = contact.Id
    left join [Framework].[ContactPhone-Active] contactPhone on smsPreference.ContactPhoneId = contactPhone.Id
    inner join [Framework].[Portal-Active] portal on smsPreference.PortalId = portal.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, smsPreference.PortalId, smsPreference.OrganizationId)
where smsPreference.Id = @Id