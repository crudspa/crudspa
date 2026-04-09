create proc [FrameworkCore].[UserUpdateSettings] (
     @SessionId uniqueidentifier
    ,@Username nvarchar(75)
    ,@FirstName nvarchar(75)
    ,@LastName nvarchar(75)
    ,@TimeZoneId nvarchar(32)
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @userId uniqueidentifier = (select top 1 UserId from [Framework].[Session-Active] where Id = @SessionId)
declare @contactId uniqueidentifier = (select top 1 ContactId from [Framework].[User-Active] where Id = @userId)

if (@userId is not null and @contactId is not null)
begin

    begin transaction

        update [Framework].[User]
        set
            Updated = @now
            ,UpdatedBy = @SessionId
            ,Username = @Username
        where Id = @userId

        update [Framework].[Contact]
        set
            Updated = @now
            ,UpdatedBy = @SessionId
            ,FirstName = @FirstName
            ,LastName = @LastName
            ,TimeZoneId = @TimeZoneId
        where Id = @contactId

    commit transaction

end