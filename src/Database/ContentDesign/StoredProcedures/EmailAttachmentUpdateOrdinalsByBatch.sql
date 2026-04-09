create proc [ContentDesign].[EmailAttachmentUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update emailAttachment
set
    emailAttachment.Ordinal = orderable.Ordinal
from [Content].[EmailAttachment] emailAttachment
    inner join @Orderables orderable on orderable.Id = emailAttachment.Id
where emailAttachment.Ordinal != orderable.Ordinal

commit transaction