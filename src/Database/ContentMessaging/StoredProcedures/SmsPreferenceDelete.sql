create proc [ContentMessaging].[SmsPreferenceDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1 from [Content].[SmsPreference-Active] preference
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, preference.PortalId, preference.OrganizationId)
    where preference.Id = @Id
)
    throw 51000, 'SMS preference access denied.', 1

begin transaction

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[SmsPreference] baseTable
    inner join [Content].[SmsPreference-Active] smsPreference on smsPreference.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction