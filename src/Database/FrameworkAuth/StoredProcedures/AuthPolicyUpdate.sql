create proc [FrameworkAuth].[AuthPolicyUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@AuthConnectionId uniqueidentifier
    ,@Audience nvarchar(25)
    ,@Key nvarchar(75)
    ,@IdleTimeoutMinutes int
    ,@AbsoluteTimeoutMinutes int
    ,@Persist bit
    ,@AutoRedirect bit
    ,@Fallback bit
    ,@Enabled bit
) as

if @Key is not null and exists (
    select 1
    from [Framework].[AuthPolicy-Active]
    where Audience = @Audience
        and [Key] = @Key
        and Id <> @Id
)
    raiserror('District Link is already in use', 16, 1)

update [Framework].[AuthPolicy]
set Updated = sysdatetimeoffset()
    ,UpdatedBy = @SessionId
    ,AuthConnectionId = @AuthConnectionId
    ,Audience = @Audience
    ,[Key] = @Key
    ,IdleTimeoutMinutes = @IdleTimeoutMinutes
    ,AbsoluteTimeoutMinutes = @AbsoluteTimeoutMinutes
    ,Persist = @Persist
    ,AutoRedirect = @AutoRedirect
    ,Fallback = @Fallback
    ,Enabled = @Enabled
where Id = @Id
    and OrganizationId = @OrganizationId
    and VersionOf = Id
    and IsDeleted = 0

if @@rowcount = 0
    raiserror('Auth policy not found', 16, 1)