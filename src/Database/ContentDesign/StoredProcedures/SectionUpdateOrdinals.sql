create proc [ContentDesign].[SectionUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update section
set
     section.Ordinal = orderable.Ordinal
    ,section.Updated = @now
    ,section.UpdatedBy = @SessionId
from [Content].[Section] section
    inner join @Orderables orderable on orderable.Id = section.Id
where section.Ordinal != orderable.Ordinal

commit transaction