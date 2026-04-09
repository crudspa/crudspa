create proc [ContentDesign].[NoteImageUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update noteImage
set
    noteImage.Ordinal = orderable.Ordinal
    ,noteImage.Updated = @now
    ,noteImage.UpdatedBy = @SessionId
from [Content].[NoteImage] noteImage
    inner join @Orderables orderable on orderable.Id = noteImage.Id
where noteImage.Ordinal != orderable.Ordinal

commit transaction