create proc [FrameworkCore].[SmsWebhookMessageUpsert] (
     @SessionId uniqueidentifier
    ,@SmsChannelKey nvarchar(50)
    ,@PortalId uniqueidentifier
    ,@Provider int
    ,@ProviderMessageId nvarchar(75)
    ,@FromNumber nvarchar(20)
    ,@ToNumber nvarchar(20)
    ,@Body nvarchar(max)
    ,@ProviderStatus nvarchar(75)
    ,@SegmentCount int
    ,@Id uniqueidentifier output
) as

declare @now datetimeoffset = sysdatetimeoffset()

select @Id = smsMessage.Id
from [Content].[SmsMessage] smsMessage
where smsMessage.Provider = @Provider
    and (smsMessage.SmsChannelKey = @SmsChannelKey or (smsMessage.SmsChannelKey is null and @SmsChannelKey is null))
    and smsMessage.ProviderMessageId = @ProviderMessageId
    and @ProviderMessageId is not null

if @Id is not null
begin
    update [Content].[SmsMessage]
    set Updated = @now
        ,UpdatedBy = @SessionId
        ,SmsChannelKey = @SmsChannelKey
        ,Body = @Body
        ,FromNumber = @FromNumber
        ,ToNumber = @ToNumber
        ,Occurred = @now
        ,Status = 4
        ,StatusUpdated = @now
        ,ProviderStatus = @ProviderStatus
        ,SegmentCount = @SegmentCount
    where Id = @Id

    return
end

declare @digits nvarchar(20) = @FromNumber
set @digits = replace(@digits, '+', '')
set @digits = replace(@digits, ' ', '')
set @digits = replace(@digits, '-', '')
set @digits = replace(@digits, '.', '')
set @digits = replace(@digits, '(', '')
set @digits = replace(@digits, ')', '')

declare @contactPhoneId uniqueidentifier
declare @contactId uniqueidentifier
declare @memberId uniqueidentifier
declare @membershipId uniqueidentifier

if @PortalId is not null
begin
    select top 1
        @contactPhoneId = contactPhone.Id
        ,@contactId = contactPhone.ContactId
    from [Framework].[ContactPhone] contactPhone
        inner join [Content].[Member] member on member.ContactId = contactPhone.ContactId
        inner join [Content].[Membership] membership on membership.Id = member.MembershipId
    where contactPhone.IsDeleted = 0
        and contactPhone.VersionOf = contactPhone.Id
        and member.VersionOf = member.Id
        and membership.VersionOf = membership.Id
        and member.IsDeleted = 0
        and membership.IsDeleted = 0
        and membership.PortalId = @PortalId
        and contactPhone.SupportsSms = 1
        and right(replace(replace(replace(replace(replace(replace(contactPhone.Phone, '+', ''), ' ', ''), '-', ''), '.', ''), '(', ''), ')', ''), 10) = right(@digits, 10)
    order by contactPhone.Updated desc
end

if @contactPhoneId is null
begin
    select top 1
        @contactPhoneId = contactPhone.Id
        ,@contactId = contactPhone.ContactId
    from [Framework].[ContactPhone] contactPhone
    where contactPhone.IsDeleted = 0
        and contactPhone.VersionOf = contactPhone.Id
        and contactPhone.SupportsSms = 1
        and right(replace(replace(replace(replace(replace(replace(contactPhone.Phone, '+', ''), ' ', ''), '-', ''), '.', ''), '(', ''), ')', ''), 10) = right(@digits, 10)
    order by contactPhone.Updated desc
end

if @contactPhoneId is null
begin
    select top 1
        @contactPhoneId = contactPhone.Id
        ,@contactId = contactPhone.ContactId
    from [Framework].[ContactPhone] contactPhone
    where contactPhone.IsDeleted = 0
        and contactPhone.VersionOf = contactPhone.Id
        and right(replace(replace(replace(replace(replace(replace(contactPhone.Phone, '+', ''), ' ', ''), '-', ''), '.', ''), '(', ''), ')', ''), 10) = right(@digits, 10)
    order by contactPhone.Updated desc
end

if @contactId is not null
begin
    select top 1
        @memberId = member.Id
        ,@membershipId = member.MembershipId
    from [Content].[Member] member
    inner join [Content].[Membership] membership on membership.Id = member.MembershipId
    where member.IsDeleted = 0
        and member.VersionOf = member.Id
        and membership.VersionOf = membership.Id
        and membership.IsDeleted = 0
        and member.ContactId = @contactId
        and (membership.PortalId = @PortalId or @PortalId is null)
    order by member.Updated desc
end

set @Id = newid()

insert [Content].[SmsMessage] (
    Id
    ,Updated
    ,UpdatedBy
    ,SmsChannelKey
    ,MembershipId
    ,MemberId
    ,ContactId
    ,ContactPhoneId
    ,Direction
    ,Body
    ,FromNumber
    ,ToNumber
    ,Occurred
    ,Status
    ,StatusUpdated
    ,Provider
    ,ProviderMessageId
    ,ProviderStatus
    ,SegmentCount
)
values (
    @Id
    ,@now
    ,@SessionId
    ,@SmsChannelKey
    ,@membershipId
    ,@memberId
    ,@contactId
    ,@contactPhoneId
    ,0
    ,@Body
    ,@FromNumber
    ,@ToNumber
    ,@now
    ,4
    ,@now
    ,@Provider
    ,@ProviderMessageId
    ,@ProviderStatus
    ,@SegmentCount
)