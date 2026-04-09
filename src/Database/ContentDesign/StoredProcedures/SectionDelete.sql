create proc [ContentDesign].[SectionDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @pageId uniqueidentifier = (select top 1 PageId from [Content].[Section] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Content].[Section-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update section
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Section] section
where section.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Content].[Section] baseTable
    inner join [Content].[Section-Active] section on section.Id = baseTable.Id
where section.PageId = @pageId
    and section.Ordinal > @oldOrdinal

commit transaction