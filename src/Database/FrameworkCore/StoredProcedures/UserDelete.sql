create proc [FrameworkCore].[UserDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update userTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Framework].[User] userTable
where userTable.Id = @Id

update userRole
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Framework].[UserRole] userRole
where userRole.UserId = @Id

commit transaction