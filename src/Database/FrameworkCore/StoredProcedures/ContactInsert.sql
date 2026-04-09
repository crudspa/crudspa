create proc [FrameworkCore].[ContactInsert] (
     @SessionId uniqueidentifier
    ,@FirstName nvarchar(75)
    ,@LastName nvarchar(75)
    ,@TimeZoneId nvarchar(32)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Framework].[Contact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,FirstName
    ,LastName
    ,TimeZoneId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@FirstName
    ,@LastName
    ,@TimeZoneId
)

commit transaction