create proc [FrameworkCore].[OrganizationUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(75)
    ,@TimeZoneId nvarchar(32)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Framework].[Organization]
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,Name = @Name
    ,TimeZoneId  = @TimeZoneId
where Id = @Id

commit transaction