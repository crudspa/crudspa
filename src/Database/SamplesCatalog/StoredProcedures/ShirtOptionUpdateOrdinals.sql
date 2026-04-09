create proc [SamplesCatalog].[ShirtOptionUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update shirtOption
set
     shirtOption.Ordinal = orderable.Ordinal
    ,shirtOption.Updated = @now
    ,shirtOption.UpdatedBy = @SessionId
from [Samples].[ShirtOption] shirtOption
    inner join @Orderables orderable on orderable.Id = shirtOption.Id
where shirtOption.Ordinal != orderable.Ordinal

commit transaction