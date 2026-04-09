create proc [SamplesCatalog].[MovieCreditDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @movieId uniqueidentifier = (
    select top 1 MovieId
    from [Samples].[MovieCredit-Active] movieCredit
    where movieCredit.Id = @Id
)

declare @oldOrdinal int = (
    select top 1 Ordinal
    from [Samples].[MovieCredit-Active] movieCredit
    where movieCredit.Id = @Id
)

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Samples].[MovieCredit] baseTable
    inner join [Samples].[MovieCredit-Active] movieCredit on movieCredit.Id = baseTable.Id
where baseTable.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Samples].[MovieCredit] baseTable
    inner join [Samples].[MovieCredit-Active] movieCredit on movieCredit.Id = baseTable.Id
where movieCredit.MovieId = @movieId
    and movieCredit.Ordinal > @oldOrdinal

commit transaction