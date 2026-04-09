create proc [SamplesCatalog].[BookEditionUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update bookEdition
set
    bookEdition.Ordinal = orderable.Ordinal
    ,bookEdition.Updated = @now
    ,bookEdition.UpdatedBy = @SessionId
from [Samples].[BookEdition] bookEdition
    inner join @Orderables orderable on orderable.Id = bookEdition.Id
where bookEdition.Ordinal != orderable.Ordinal

commit transaction