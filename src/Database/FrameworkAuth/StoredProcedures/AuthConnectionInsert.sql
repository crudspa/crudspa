create proc [FrameworkAuth].[AuthConnectionInsert] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@Provider nvarchar(75)
    ,@Tenant nvarchar(255)
    ,@Enabled bit
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[AuthConnection] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,OrganizationId
    ,Provider
    ,Tenant
    ,Enabled
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@OrganizationId
    ,@Provider
    ,@Tenant
    ,@Enabled
)