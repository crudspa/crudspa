create proc [FrameworkAuth].[AuthConnectionUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@Provider nvarchar(75)
    ,@Tenant nvarchar(255)
    ,@Enabled bit
) as

update [Framework].[AuthConnection]
set Updated = sysdatetimeoffset()
    ,UpdatedBy = @SessionId
    ,Provider = @Provider
    ,Tenant = @Tenant
    ,Enabled = @Enabled
where Id = @Id
    and OrganizationId = @OrganizationId
    and VersionOf = Id
    and IsDeleted = 0

if @@rowcount = 0
    raiserror('Auth connection not found', 16, 1)