create proc [EducationPublisher].[GameActivityUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update gameActivity
set
     gameActivity.Ordinal = orderable.Ordinal
    ,gameActivity.Updated = @now
    ,gameActivity.UpdatedBy = @SessionId
from [Education].[GameActivity] gameActivity
    inner join @Orderables orderable on orderable.Id = gameActivity.Id
where gameActivity.Ordinal != orderable.Ordinal

commit transaction