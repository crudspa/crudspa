create proc [ContentDesign].[ElementDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @sectionId uniqueidentifier = (select top 1 SectionId from [Content].[Element] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Content].[Element-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update element
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Element] element
where element.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Content].[Element] baseTable
    inner join [Content].[Element-Active] element on element.Id = baseTable.Id
where element.SectionId = @sectionId
    and element.Ordinal > @oldOrdinal

commit transaction