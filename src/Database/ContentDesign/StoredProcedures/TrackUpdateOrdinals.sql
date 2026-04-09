create proc [ContentDesign].[TrackUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update track
set
     track.Ordinal = orderable.Ordinal
    ,track.Updated = @now
    ,track.UpdatedBy = @SessionId
from [Content].[Track] track
    inner join @Orderables orderable on orderable.Id = track.Id
where track.Ordinal != orderable.Ordinal

commit transaction