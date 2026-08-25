create proc [ContentMessaging].[SmsAttachmentUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@SmsId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[SmsAttachment-Active] attachment
        inner join [Content].[Sms-Active] sms on attachment.SmsId = sms.Id
        inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where attachment.Id = @Id and attachment.SmsId = @SmsId
)
    throw 51000, 'SMS attachment access denied.', 1

begin transaction

update [Content].[SmsAttachment]
set
    Id = @Id
    ,ImageId = @ImageId
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction