create proc [ContentDesign].[MultimediaItemUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update multimediaItem
set
    multimediaItem.Ordinal = orderable.Ordinal
    ,multimediaItem.Updated = @now
    ,multimediaItem.UpdatedBy = @SessionId
from [Content].[MultimediaItem] multimediaItem
    inner join @Orderables orderable on orderable.Id = multimediaItem.Id
where multimediaItem.Ordinal != orderable.Ordinal

commit transaction