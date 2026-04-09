create proc [SamplesCatalog].[CatalogContactDelete] (
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
from [Samples].[CatalogContact] baseTable
    inner join [Samples].[CatalogContact-Active] catalogContact on catalogContact.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction