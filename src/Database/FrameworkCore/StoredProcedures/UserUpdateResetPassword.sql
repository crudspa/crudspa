create proc [FrameworkCore].[UserUpdateResetPassword] (
     @SessionId uniqueidentifier
    ,@UserId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[User]
set ResetPassword = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @UserId