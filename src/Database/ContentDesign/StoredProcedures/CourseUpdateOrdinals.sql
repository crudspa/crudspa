create proc [ContentDesign].[CourseUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update course
set
     course.Ordinal = orderable.Ordinal
    ,course.Updated = @now
    ,course.UpdatedBy = @SessionId
from [Content].[Course] course
    inner join @Orderables orderable on orderable.Id = course.Id
where course.Ordinal != orderable.Ordinal

commit transaction