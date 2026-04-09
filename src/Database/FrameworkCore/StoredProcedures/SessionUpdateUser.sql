create proc [FrameworkCore].[SessionUpdateUser] (
     @Id uniqueidentifier
    ,@UserId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[Session]
set UserId = @UserId
    ,UserAdded = @now
Where Id = @Id