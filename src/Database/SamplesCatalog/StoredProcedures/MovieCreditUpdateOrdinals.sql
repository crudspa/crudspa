create proc [SamplesCatalog].[MovieCreditUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update movieCredit
set
     movieCredit.Ordinal = orderable.Ordinal
    ,movieCredit.Updated = @now
    ,movieCredit.UpdatedBy = @SessionId
from [Samples].[MovieCredit] movieCredit
    inner join @Orderables orderable on orderable.Id = movieCredit.Id
where movieCredit.Ordinal != orderable.Ordinal

commit transaction