create proc [FrameworkAuth].[SessionAuthRevoke] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Reason nvarchar(75)
) as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()
declare @revoked bit = 0

if nullif(ltrim(rtrim(@Reason)), N'') is null
begin
    select @revoked
    return
end

begin transaction

if exists (
    select 1
    from [Framework].[SessionAuth] sessionAuth with (updlock, holdlock)
        inner join [Framework].[Session-Active] session on sessionAuth.SessionId = session.Id
    where sessionAuth.SessionId = @SessionId
        and session.PortalId = @PortalId
)
begin
    update [Framework].[SessionAuth]
    set Revoked = coalesce(Revoked, @now)
        ,RevocationReason = coalesce(RevocationReason, @Reason)
    where SessionId = @SessionId

    exec [FrameworkCore].[SessionEnd] @SessionId

    insert [Framework].[AuthEvent] (
         Id
        ,Created
        ,CorrelationId
        ,Type
        ,Outcome
        ,Provider
        ,Tenant
        ,Audience
        ,PortalId
        ,ExternalIdentityId
        ,SessionId
        ,Reason
    )
    select
         newid()
        ,@now
        ,@SessionId
        ,N'session-revoked'
        ,N'succeeded'
        ,sessionAuth.Provider
        ,externalIdentity.Tenant
        ,portal.[Key]
        ,session.PortalId
        ,sessionAuth.ExternalIdentityId
        ,@SessionId
        ,@Reason
    from [Framework].[SessionAuth] sessionAuth
        inner join [Framework].[Session] session on sessionAuth.SessionId = session.Id
        left join [Framework].[ExternalIdentity] externalIdentity on sessionAuth.ExternalIdentityId = externalIdentity.Id
        left join [Framework].[Portal] portal on session.PortalId = portal.Id
    where sessionAuth.SessionId = @SessionId

    set @revoked = 1
end

commit transaction

select @revoked