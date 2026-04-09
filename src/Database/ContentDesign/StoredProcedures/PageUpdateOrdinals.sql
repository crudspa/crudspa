create proc [ContentDesign].[PageUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update page
set
     page.Ordinal = orderable.Ordinal
    ,page.Updated = @now
    ,page.UpdatedBy = @SessionId
from [Content].[Page] page
    inner join @Orderables orderable on orderable.Id = page.Id
where page.Ordinal != orderable.Ordinal

commit transaction