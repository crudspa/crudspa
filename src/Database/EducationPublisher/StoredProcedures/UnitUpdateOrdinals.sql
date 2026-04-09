create proc [EducationPublisher].[UnitUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update unit
set
     unit.Ordinal = orderable.Ordinal
    ,unit.Updated = @now
    ,unit.UpdatedBy = @SessionId
from [Education].[Unit] unit
    inner join @Orderables orderable on orderable.Id = unit.Id
where unit.Ordinal != orderable.Ordinal

commit transaction