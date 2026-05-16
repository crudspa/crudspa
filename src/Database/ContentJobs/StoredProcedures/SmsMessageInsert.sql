create proc [ContentJobs].[SmsMessageInsert] (
     @SessionId uniqueidentifier
    ,@SmsId uniqueidentifier
    ,@SmsChannelKey nvarchar(50)
    ,@MembershipId uniqueidentifier
    ,@MemberId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@ContactPhoneId uniqueidentifier
    ,@Body nvarchar(max)
    ,@FromNumber nvarchar(20)
    ,@ToNumber nvarchar(20)
    ,@Status int
    ,@Provider int
    ,@ProviderMessageId nvarchar(75)
    ,@ApiResponse nvarchar(max)
) as

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
     newid()
    ,@SessionId
    ,@SmsId
    ,@SmsChannelKey
    ,@MembershipId
    ,@MemberId
    ,@ContactId
    ,@ContactPhoneId
    ,1
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