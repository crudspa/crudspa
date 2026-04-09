create proc [EducationPublisher].[ListenChoiceUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update listenChoice
set
    listenChoice.Ordinal = orderable.Ordinal
    ,listenChoice.Updated = @now
    ,listenChoice.UpdatedBy = @SessionId
from [Education].[ListenChoice] listenChoice
    inner join @Orderables orderable on orderable.Id = listenChoice.Id
where listenChoice.Ordinal != orderable.Ordinal

commit transaction