create proc [ContentJobs].[MemberSelectForSending] (
     @MembershipId uniqueidentifier
    ,@SmsChannelKey nvarchar(50) = null
) as

set nocount on

select
     member.Id
    ,member.MembershipId
    ,member.Status
    ,contact.Id as ContactId
    ,contact.FirstName as ContactFirstName
    ,contact.LastName as ContactLastName
    ,contactEmail.Email as ContactEmailEmail
    ,contactPhone.Id as ContactPhoneId
    ,contactPhone.Phone as ContactPhonePhone
    ,contactPhone.SupportsSms as ContactPhoneSupportsSms
from [Content].[Member-Active] member
    inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
    inner join [Framework].[Contact-Active] contact on member.ContactId = contact.Id
    outer apply (
        select top (1) email.Email
        from [Framework].[ContactEmail-Active] email
        where email.ContactId = contact.Id
        order by email.Ordinal
    ) contactEmail
    outer apply (
        select top (1) phone.Id, phone.Phone, phone.SupportsSms
        from [Framework].[ContactPhone-Active] phone
        where phone.ContactId = contact.Id
            and phone.SupportsSms = 1
            and not exists (
                select 1
                from [Content].[SmsPreference-Active] preference
                where preference.PortalId = membership.PortalId
                    and preference.Status in (2, 3)
                    and (preference.SmsChannelKey = @SmsChannelKey or preference.SmsChannelKey is null or @SmsChannelKey is null)
                    and (
                        preference.ContactPhoneId = phone.Id
                        or right(replace(replace(replace(replace(replace(replace(preference.Number, '+', ''), ' ', ''), '-', ''), '.', ''), '(', ''), ')', ''), 10)
                            = right(replace(replace(replace(replace(replace(replace(phone.Phone, '+', ''), ' ', ''), '-', ''), '.', ''), '(', ''), ')', ''), 10)
                    )
            )
        order by phone.Ordinal
    ) contactPhone
where membership.Id = @MembershipId
    and (member.Status = 0 or member.Status = 1)

select
     tokenValue.Id
    ,tokenValue.TokenId
    ,tokenValue.ContactId
    ,tokenValue.Value
    ,token.[Key] as TokenKey
from [Content].[TokenValue-Active] tokenValue
    inner join [Content].[Token-Active] token on tokenValue.TokenId = token.Id
    inner join [Framework].[Contact-Active] contact on tokenValue.ContactId = contact.Id
    inner join [Content].[Member-Active] member on member.ContactId = contact.Id
    inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
where membership.Id = @MembershipId
    and token.MembershipId = @MembershipId
    and (member.Status = 0 or member.Status = 1)