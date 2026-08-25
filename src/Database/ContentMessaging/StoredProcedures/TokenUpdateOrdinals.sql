create proc [ContentMessaging].[TokenUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if exists (
    select 1
    from @Orderables orderable
        inner join [Content].[Token-Active] token on orderable.Id = token.Id
        inner join [Content].[Membership-Active] membership on token.MembershipId = membership.Id
    where not exists (select 1 from [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId))
)
    or exists (select 1 from @Orderables orderable where not exists (select 1 from [Content].[Token-Active] token where token.Id = orderable.Id))
    throw 51000, 'Token access denied.', 1

begin transaction

update token
set
     token.Ordinal = orderable.Ordinal
from [Content].[Token] token
    inner join @Orderables orderable on orderable.Id = token.Id
where token.Ordinal != orderable.Ordinal

commit transaction