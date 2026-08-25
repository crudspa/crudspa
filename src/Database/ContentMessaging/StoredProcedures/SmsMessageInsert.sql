create proc [ContentMessaging].[SmsMessageInsert] (
     @SessionId uniqueidentifier
    ,@SmsId uniqueidentifier
    ,@SmsChannelKey nvarchar(50)
    ,@MembershipId uniqueidentifier
    ,@MemberId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@ContactPhoneId uniqueidentifier
    ,@Direction int
    ,@Body nvarchar(max)
    ,@FromNumber nvarchar(20)
    ,@ToNumber nvarchar(20)
    ,@Status int
    ,@Provider int
    ,@ProviderMessageId nvarchar(75)
    ,@ApiResponse nvarchar(max)
    ,@Id uniqueidentifier output
) as

if not exists (
    select 1
    from [Content].[Membership-Active] membership
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where membership.Id = @MembershipId
        and (@MemberId is null or exists (select 1 from [Content].[Member-Active] where Id = @MemberId and MembershipId = @MembershipId))
)
    throw 51000, 'SMS message access denied.', 1

set @Id = newid()

insert [Content].[SmsMessage] (
     Id
    ,UpdatedBy
    ,SmsId
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
    ,ApiResponse
) values (
     @Id
    ,@SessionId
    ,@SmsId
    ,@SmsChannelKey
    ,@MembershipId
    ,@MemberId
    ,@ContactId
    ,@ContactPhoneId
    ,@Direction
    ,@Body
    ,@FromNumber
    ,@ToNumber
    ,sysdatetimeoffset()
    ,@Status
    ,sysdatetimeoffset()
    ,@Provider
    ,@ProviderMessageId
    ,@ApiResponse
)