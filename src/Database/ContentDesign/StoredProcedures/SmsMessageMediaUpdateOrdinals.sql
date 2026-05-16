create proc [ContentDesign].[SmsMessageMediaUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
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