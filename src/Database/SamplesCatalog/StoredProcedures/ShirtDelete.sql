create proc [SamplesCatalog].[ShirtDelete] (
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
from [Samples].[Shirt] baseTable
    inner join [Samples].[Shirt-Active] shirt on shirt.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction