create proc [FrameworkCore].[ContactDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update contact
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Framework].[Contact] contact
where contact.Id = @Id

commit transaction