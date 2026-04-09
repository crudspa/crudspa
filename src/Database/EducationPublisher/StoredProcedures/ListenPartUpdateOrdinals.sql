create proc [EducationPublisher].[ListenPartUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update listenPart
set
     listenPart.Ordinal = orderable.Ordinal
    ,listenPart.Updated = @now
    ,listenPart.UpdatedBy = @SessionId
from [Education].[ListenPart] listenPart
    inner join @Orderables orderable on orderable.Id = listenPart.Id
where listenPart.Ordinal != orderable.Ordinal

commit transaction