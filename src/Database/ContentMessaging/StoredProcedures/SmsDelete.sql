create proc [ContentMessaging].[SmsDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[Sms-Active] sms
        inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where sms.Id = @Id
)
    throw 51000, 'SMS access denied.', 1

begin transaction

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Sms] baseTable
    inner join [Content].[Sms-Active] sms on sms.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction