create proc [ContentDesign].[CommentMediaUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update commentMedia
set
    commentMedia.Ordinal = orderable.Ordinal
    ,commentMedia.Updated = @now
    ,commentMedia.UpdatedBy = @SessionId
from [Content].[CommentMedia] commentMedia
    inner join @Orderables orderable on orderable.Id = commentMedia.Id
where commentMedia.Ordinal != orderable.Ordinal

commit transaction