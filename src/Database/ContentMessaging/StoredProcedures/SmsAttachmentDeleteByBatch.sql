create proc [ContentMessaging].[SmsAttachmentDeleteByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set xact_abort on
set nocount on

if not exists (
    select 1
    from [Content].[SmsAttachment-Active] attachment
        inner join [Content].[Sms-Active] sms on attachment.SmsId = sms.Id
        inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where attachment.Id = @Id
)
    throw 51000, 'SMS attachment access denied.', 1

begin transaction

update [Content].[SmsAttachment]
set  IsDeleted = 1
where Id = @Id

commit transaction