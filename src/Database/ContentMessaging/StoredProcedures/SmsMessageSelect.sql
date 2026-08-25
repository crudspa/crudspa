create proc [ContentMessaging].[SmsMessageSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     smsMessage.Id
    ,smsMessage.MembershipId
    ,coalesce(membership.PortalId, smsMembership.PortalId, memberMembership.PortalId) as PortalId
    ,smsMessage.SmsId
    ,smsMessage.SmsChannelKey
    ,smsMessage.MemberId
    ,smsMessage.Body
    ,smsMessage.Direction
    ,smsMessage.Occurred
    ,smsMessage.FromNumber
    ,smsMessage.ToNumber
    ,smsMessage.Status
    ,smsMessage.ProviderMessageId
    ,smsMessage.Provider
    ,smsMessage.ApiResponse
    ,smsMessage.ContactPhoneId
    ,smsMessage.ContactId
    ,contact.FirstName as ContactFirstName
    ,contact.LastName as ContactLastName
from [Content].[SmsMessage-Active] smsMessage
    left join [Content].[Membership-Active] membership on smsMessage.MembershipId = membership.Id
    left join [Content].[Sms-Active] sms on smsMessage.SmsId = sms.Id
    left join [Content].[Membership-Active] smsMembership on sms.MembershipId = smsMembership.Id
    left join [Content].[Member-Active] member on smsMessage.MemberId = member.Id
    left join [Content].[Membership-Active] memberMembership on member.MembershipId = memberMembership.Id
    left join [Framework].[Contact-Active] contact on smsMessage.ContactId = contact.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](
        @SessionId,
        coalesce(membership.PortalId, smsMembership.PortalId, memberMembership.PortalId),
        coalesce(membership.OrganizationId, smsMembership.OrganizationId, memberMembership.OrganizationId)
    )
where smsMessage.Id = @Id