create proc [ContentMessaging].[SmsAttachmentUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if exists (
    select 1
    from @Orderables orderable
        inner join [Content].[SmsAttachment-Active] attachment on orderable.Id = attachment.Id
        inner join [Content].[Sms-Active] sms on attachment.SmsId = sms.Id
        inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
    where not exists (select 1 from [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId))
)
    or exists (select 1 from @Orderables orderable where not exists (select 1 from [Content].[SmsAttachment-Active] attachment where attachment.Id = orderable.Id))
    throw 51000, 'SMS attachment access denied.', 1

begin transaction

update smsAttachment
set
    smsAttachment.Ordinal = orderable.Ordinal
from [Content].[SmsAttachment] smsAttachment
    inner join @Orderables orderable on orderable.Id = smsAttachment.Id
where smsAttachment.Ordinal != orderable.Ordinal

commit transaction