create proc [SamplesCatalog].[ShirtOptionDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @shirtId uniqueidentifier = (
    select top 1 ShirtId
    from [Samples].[ShirtOption-Active] shirtOption
    where shirtOption.Id = @Id
)

declare @oldOrdinal int = (
    select top 1 Ordinal
    from [Samples].[ShirtOption-Active] shirtOption
    where shirtOption.Id = @Id
)

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Samples].[ShirtOption] baseTable
    inner join [Samples].[ShirtOption-Active] shirtOption on shirtOption.Id = baseTable.Id
where baseTable.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Samples].[ShirtOption] baseTable
    inner join [Samples].[ShirtOption-Active] shirtOption on shirtOption.Id = baseTable.Id
where shirtOption.ShirtId = @shirtId
    and shirtOption.Ordinal > @oldOrdinal

commit transaction