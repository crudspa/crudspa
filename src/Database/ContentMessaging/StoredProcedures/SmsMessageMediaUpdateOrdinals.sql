create proc [ContentMessaging].[SmsMessageMediaUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if exists (
    select 1
    from @Orderables orderable
        inner join [Content].[SmsMessageMedia-Active] media on orderable.Id = media.Id
        inner join [Content].[SmsMessage-Active] message on media.SmsMessageId = message.Id
        inner join [Content].[Membership-Active] membership on message.MembershipId = membership.Id
    where not exists (select 1 from [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId))
)
    or exists (select 1 from @Orderables orderable where not exists (select 1 from [Content].[SmsMessageMedia-Active] media where media.Id = orderable.Id))
    throw 51000, 'SMS media access denied.', 1

begin transaction

update smsMessageMedia
set
     smsMessageMedia.Ordinal = orderable.Ordinal
    ,smsMessageMedia.Updated = @now
    ,smsMessageMedia.UpdatedBy = @SessionId
from [Content].[SmsMessageMedia] smsMessageMedia
    inner join @Orderables orderable on orderable.Id = smsMessageMedia.Id
where smsMessageMedia.Ordinal != orderable.Ordinal

commit transaction