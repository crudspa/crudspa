create proc [EducationPublisher].[LessonUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update lesson
set
     lesson.Ordinal = orderable.Ordinal
    ,lesson.Updated = @now
    ,lesson.UpdatedBy = @SessionId
from [Education].[Lesson] lesson
    inner join @Orderables orderable on orderable.Id = lesson.Id
where lesson.Ordinal != orderable.Ordinal

commit transaction