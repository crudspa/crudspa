create proc [ContentMessaging].[SmsAttachmentInsertByBatch] (
     @SessionId uniqueidentifier
    ,@SmsId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[Sms-Active] sms
        inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where sms.Id = @SmsId
)
    throw 51000, 'SMS attachment access denied.', 1

begin transaction

insert [Content].[SmsAttachment] (
     Id
    ,SmsId
    ,ImageId
    ,Ordinal
)
values (
     @Id
    ,@SmsId
    ,@ImageId
    ,@Ordinal
)

commit transaction