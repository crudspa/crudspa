create proc [EducationPublisher].[ReadPartUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update readPart
set
     readPart.Ordinal = orderable.Ordinal
    ,readPart.Updated = @now
    ,readPart.UpdatedBy = @SessionId
from [Education].[ReadPart] readPart
    inner join @Orderables orderable on orderable.Id = readPart.Id
where readPart.Ordinal != orderable.Ordinal

commit transaction