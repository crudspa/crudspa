create proc [ContentMessaging].[EmailAttachmentUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if exists (
    select 1
    from @Orderables orderable
        inner join [Content].[EmailAttachment-Active] attachment on orderable.Id = attachment.Id
        inner join [Content].[Email-Active] email on attachment.EmailId = email.Id
        inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
    where not exists (select 1 from [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId))
)
    or exists (select 1 from @Orderables orderable where not exists (select 1 from [Content].[EmailAttachment-Active] attachment where attachment.Id = orderable.Id))
    throw 51000, 'Email attachment access denied.', 1

begin transaction

update emailAttachment
set
    emailAttachment.Ordinal = orderable.Ordinal
from [Content].[EmailAttachment] emailAttachment
    inner join @Orderables orderable on orderable.Id = emailAttachment.Id
where emailAttachment.Ordinal != orderable.Ordinal

commit transaction