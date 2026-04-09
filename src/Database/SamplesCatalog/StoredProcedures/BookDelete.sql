create proc [SamplesCatalog].[BookDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Samples].[Book] baseTable
    inner join [Samples].[Book-Active] book on book.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction