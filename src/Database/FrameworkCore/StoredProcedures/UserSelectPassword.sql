create proc [FrameworkCore].[UserSelectPassword] (
     @SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

select distinct userTable.Id
    ,userTable.Username
    ,userTable.PasswordHash
    ,userTable.PasswordSalt
    ,userTable.ResetPassword
from [Framework].[User-Active] userTable
where userTable.Id = (select top 1 UserId from [Framework].[Session-Active] where Id = @SessionId)