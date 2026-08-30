create proc [FrameworkAuth].[AuthPolicyInsert] (
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

declare @now datetimeoffset = sysdatetimeoffset()

if @Key is not null and exists (
    select 1
    from [Framework].[AuthPolicy-Active]
    where Audience = @Audience
        and [Key] = @Key
        and Id <> @Id
)
    raiserror('District Link is already in use', 16, 1)

insert [Framework].[AuthPolicy] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,OrganizationId
    ,AuthConnectionId
    ,Audience
    ,[Key]
    ,IdleTimeoutMinutes
    ,AbsoluteTimeoutMinutes
    ,Persist
    ,AutoRedirect
    ,Fallback
    ,Enabled
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@OrganizationId
    ,@AuthConnectionId
    ,@Audience
    ,@Key
    ,@IdleTimeoutMinutes
    ,@AbsoluteTimeoutMinutes
    ,@Persist
    ,@AutoRedirect
    ,@Fallback
    ,@Enabled
)