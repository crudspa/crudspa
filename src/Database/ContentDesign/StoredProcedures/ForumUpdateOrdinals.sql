create proc [ContentDesign].[ForumUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update forum
set
     forum.Ordinal = orderable.Ordinal
    ,forum.Updated = @now
    ,forum.UpdatedBy = @SessionId
from [Content].[Forum] forum
    inner join @Orderables orderable on orderable.Id = forum.Id
where forum.Ordinal != orderable.Ordinal

commit transaction