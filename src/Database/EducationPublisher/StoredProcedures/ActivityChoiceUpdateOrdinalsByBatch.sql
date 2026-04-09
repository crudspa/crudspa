create proc [EducationPublisher].[ActivityChoiceUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update activityChoice
set
    activityChoice.Ordinal = orderable.Ordinal
    ,activityChoice.Updated = @now
    ,activityChoice.UpdatedBy = @SessionId
from [Education].[ActivityChoice] activityChoice
    inner join @Orderables orderable on orderable.Id = activityChoice.Id
where activityChoice.Ordinal != orderable.Ordinal

commit transaction