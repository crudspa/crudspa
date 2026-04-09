create proc [FrameworkCore].[ContactUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@FirstName nvarchar(75)
    ,@LastName nvarchar(75)
    ,@TimeZoneId nvarchar(32)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Framework].[Contact]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,FirstName = @FirstName
    ,LastName = @LastName
    ,TimeZoneId = @TimeZoneId
where Id = @Id

commit transaction