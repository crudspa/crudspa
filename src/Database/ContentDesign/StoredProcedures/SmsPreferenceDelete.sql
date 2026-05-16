create proc [ContentDesign].[SmsPreferenceDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[SmsPreference] baseTable
    inner join [Content].[SmsPreference-Active] smsPreference on smsPreference.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction