create proc [SamplesCatalog].[MovieDelete] (
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
from [Samples].[Movie] baseTable
    inner join [Samples].[Movie-Active] movie on movie.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction