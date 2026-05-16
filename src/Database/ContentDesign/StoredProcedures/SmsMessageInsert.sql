create proc [ContentDesign].[SmsMessageInsert] (
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