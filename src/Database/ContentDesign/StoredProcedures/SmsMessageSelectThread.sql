create proc [ContentDesign].[SmsMessageSelectThread] (
     @SessionId uniqueidentifier
    ,@SmsMessageId uniqueidentifier
) as

set nocount on

declare @contactPhoneId uniqueidentifier
declare @smsChannelKey nvarchar(50)
declare @counterpartyNumber nvarchar(20)
declare @channelNumber nvarchar(20)

select
     @contactPhoneId = smsMessage.ContactPhoneId
    ,@smsChannelKey = smsMessage.SmsChannelKey
    ,@counterpartyNumber = case when smsMessage.Direction = 0 then smsMessage.FromNumber else smsMessage.ToNumber end
    ,@channelNumber = case when smsMessage.Direction = 0 then smsMessage.ToNumber else smsMessage.FromNumber end
from [Content].[SmsMessage-Active] smsMessage
where smsMessage.Id = @SmsMessageId

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
where (
        @contactPhoneId is not null
        and smsMessage.ContactPhoneId = @contactPhoneId
    )
    or (
        @contactPhoneId is null
        and (smsMessage.SmsChannelKey = @smsChannelKey or (smsMessage.SmsChannelKey is null and @smsChannelKey is null))
        and (
            (smsMessage.FromNumber = @counterpartyNumber and smsMessage.ToNumber = @channelNumber)
            or (smsMessage.FromNumber = @channelNumber and smsMessage.ToNumber = @counterpartyNumber)
        )
    )
order by smsMessage.Occurred