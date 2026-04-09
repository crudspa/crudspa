create proc [EducationPublisher].[ObjectiveUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update objective
set
     objective.Ordinal = orderable.Ordinal
    ,objective.Updated = @now
    ,objective.UpdatedBy = @SessionId
from [Education].[Objective] objective
    inner join @Orderables orderable on orderable.Id = objective.Id
where objective.Ordinal != orderable.Ordinal

commit transaction