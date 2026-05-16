create proc [ContentDesign].[SmsAttachmentUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update smsAttachment
set
    smsAttachment.Ordinal = orderable.Ordinal
from [Content].[SmsAttachment] smsAttachment
    inner join @Orderables orderable on orderable.Id = smsAttachment.Id
where smsAttachment.Ordinal != orderable.Ordinal

commit transaction