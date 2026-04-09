create proc [ContentDesign].[PageDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @binderId uniqueidentifier = (select top 1 BinderId from [Content].[Page] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Content].[Page-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update page
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Page] page
where page.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Content].[Page] baseTable
    inner join [Content].[Page-Active] page on page.Id = baseTable.Id
where page.BinderId = @binderId
    and page.Ordinal > @oldOrdinal

commit transaction