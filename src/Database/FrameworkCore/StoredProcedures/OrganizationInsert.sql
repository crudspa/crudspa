create proc [FrameworkCore].[OrganizationInsert] (
     @SessionId uniqueidentifier
    ,@Name nvarchar(75)
    ,@TimeZoneId nvarchar(32)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[Organization] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Name
    ,TimeZoneId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Name
    ,@TimeZoneId
)