create proc [EducationPublisher].[ReadChoiceUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update readChoice
set
    readChoice.Ordinal = orderable.Ordinal
    ,readChoice.Updated = @now
    ,readChoice.UpdatedBy = @SessionId
from [Education].[ReadChoice] readChoice
    inner join @Orderables orderable on orderable.Id = readChoice.Id
where readChoice.Ordinal != orderable.Ordinal

commit transaction