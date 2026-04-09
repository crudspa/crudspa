create proc [SamplesCatalog].[CatalogContactUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@UserId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,UserId = @UserId
from [Samples].[CatalogContact] baseTable
    inner join [Samples].[CatalogContact-Active] catalogContact on catalogContact.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction