create proc [ContentMessaging].[SmsPreferenceUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@ContactPhoneId uniqueidentifier
    ,@Number nvarchar(20)
    ,@Status int
    ,@Source int
    ,@StatusChanged datetimeoffset(7)
    ,@Notes nvarchar(max)
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
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,ContactId = @ContactId
    ,ContactPhoneId = @ContactPhoneId
    ,Number = @Number
    ,Status = @Status
    ,Source = @Source
    ,StatusChanged = @StatusChanged
    ,Notes = @Notes
from [Content].[SmsPreference] baseTable
    inner join [Content].[SmsPreference-Active] smsPreference on smsPreference.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction