create proc [EducationPublisher].[UnitBookUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update unitBook
set
     unitBook.Ordinal = orderable.Ordinal
    ,unitBook.Updated = @now
    ,unitBook.UpdatedBy = @SessionId
from [Education].[UnitBook] unitBook
    inner join @Orderables orderable on orderable.Id = unitBook.Id
where unitBook.Ordinal != orderable.Ordinal

commit transaction