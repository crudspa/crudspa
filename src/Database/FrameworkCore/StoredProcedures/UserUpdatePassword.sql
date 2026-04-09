create proc [FrameworkCore].[UserUpdatePassword] (
     @SessionId uniqueidentifier
    ,@PasswordHash binary(32)
    ,@PasswordSalt binary(32)
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[User]
set
    Updated = @now
    ,UpdatedBy = @SessionId
    ,PasswordHash = @PasswordHash
    ,PasswordSalt = @PasswordSalt
    ,ResetPassword = 0
where Id = (select top 1 UserId from [Framework].[Session-Active] where Id = @SessionId)