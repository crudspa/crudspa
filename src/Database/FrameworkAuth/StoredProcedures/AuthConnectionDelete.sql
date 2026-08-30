create proc [FrameworkAuth].[AuthConnectionDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@OrganizationId uniqueidentifier
) as

update [Framework].[AuthConnection]
set Updated = sysdatetimeoffset()
    ,UpdatedBy = @SessionId
    ,IsDeleted = 1
where Id = @Id
    and OrganizationId = @OrganizationId
    and VersionOf = Id
    and IsDeleted = 0

if @@rowcount = 0
    raiserror('Auth connection not found', 16, 1)