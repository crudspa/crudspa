create proc [EducationPublisher].[ChapterUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update chapter
set
     chapter.Ordinal = orderable.Ordinal
    ,chapter.Updated = @now
    ,chapter.UpdatedBy = @SessionId
from [Education].[Chapter] chapter
    inner join @Orderables orderable on orderable.Id = chapter.Id
where chapter.Ordinal != orderable.Ordinal

commit transaction