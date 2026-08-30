create proc [FrameworkAuth].[SessionAuthValidate] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@LastActivity datetimeoffset
) as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()
declare @authId uniqueidentifier
declare @authPolicyId uniqueidentifier
declare @idleTimeoutMinutes int
declare @absoluteExpires datetimeoffset
declare @idleExpires datetimeoffset
declare @storedActivity datetimeoffset

begin transaction

select
     @authId = sessionAuth.Id
    ,@authPolicyId = sessionAuth.AuthPolicyId
    ,@idleTimeoutMinutes = sessionAuth.IdleTimeoutMinutes
    ,@absoluteExpires = sessionAuth.AbsoluteExpires
    ,@idleExpires = sessionAuth.IdleExpires
    ,@storedActivity = sessionAuth.LastActivity
from [Framework].[SessionAuth] sessionAuth with (updlock, holdlock)
    inner join [Framework].[Session-Active] session on sessionAuth.SessionId = session.Id
        and session.Ended is null
    inner join [Framework].[Portal-Active] portal on session.PortalId = portal.Id
    inner join [Framework].[User-Active] [user] on session.UserId = [user].Id
        and [user].PortalId = session.PortalId
    inner join [Framework].[AuthPolicy-Active] policy on sessionAuth.AuthPolicyId = policy.Id
        and policy.Audience collate Latin1_General_100_BIN2 = portal.[Key] collate Latin1_General_100_BIN2
        and policy.Enabled = 1
    inner join [Framework].[AuthConnection-Active] connection on policy.AuthConnectionId = connection.Id
        and policy.OrganizationId = connection.OrganizationId
        and connection.Provider collate Latin1_General_100_BIN2 = sessionAuth.Provider collate Latin1_General_100_BIN2
        and connection.Enabled = 1
    left join [Framework].[ExternalIdentity-Active] externalIdentity on sessionAuth.ExternalIdentityId = externalIdentity.Id
        and externalIdentity.Provider collate Latin1_General_100_BIN2 = sessionAuth.Provider collate Latin1_General_100_BIN2
        and externalIdentity.Tenant collate Latin1_General_100_BIN2 = connection.Tenant collate Latin1_General_100_BIN2
        and externalIdentity.Enabled = 1
    left join [Framework].[ExternalIdentityLink-Active] link on sessionAuth.ExternalIdentityId = link.ExternalIdentityId
        and session.UserId = link.UserId
where sessionAuth.SessionId = @SessionId
    and session.PortalId = @PortalId
    and sessionAuth.Revoked is null
    and dateadd(minute, sessionAuth.IdleTimeoutMinutes, case
        when @LastActivity > sessionAuth.LastActivity and @LastActivity <= @now then @LastActivity
        else sessionAuth.LastActivity
    end) > @now
    and sessionAuth.AbsoluteExpires > @now
    and (sessionAuth.ExternalIdentityId is null or (externalIdentity.Id is not null and link.Id is not null))

if @authId is not null
begin
    if @LastActivity > @storedActivity
    begin
        if @LastActivity > @now
            set @LastActivity = @now

        set @idleExpires = case
            when dateadd(minute, @idleTimeoutMinutes, @LastActivity) < @absoluteExpires then dateadd(minute, @idleTimeoutMinutes, @LastActivity)
            else @absoluteExpires
        end

        update [Framework].[SessionAuth]
        set LastActivity = @LastActivity
            ,IdleExpires = @idleExpires
        where Id = @authId
    end
    else
        set @LastActivity = @storedActivity
end
else
begin
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
        ,N'validation-failed'
    from [Framework].[SessionAuth] sessionAuth
        inner join [Framework].[Session] session on sessionAuth.SessionId = session.Id
        left join [Framework].[ExternalIdentity] externalIdentity on sessionAuth.ExternalIdentityId = externalIdentity.Id
        left join [Framework].[Portal] portal on session.PortalId = portal.Id
    where sessionAuth.SessionId = @SessionId
        and session.PortalId = @PortalId
        and sessionAuth.Revoked is null

    update sessionAuth
    set Revoked = coalesce(sessionAuth.Revoked, @now)
        ,RevocationReason = coalesce(sessionAuth.RevocationReason, N'validation-failed')
    from [Framework].[SessionAuth] sessionAuth
        inner join [Framework].[Session] session on sessionAuth.SessionId = session.Id
    where sessionAuth.SessionId = @SessionId
        and session.PortalId = @PortalId

    if exists (
        select 1
        from [Framework].[Session]
        where Id = @SessionId
            and PortalId = @PortalId
    )
        exec [FrameworkCore].[SessionEnd] @SessionId
end

commit transaction

if @authId is not null
    select
         @authPolicyId as AuthPolicyId
        ,@LastActivity as LastActivity
        ,@idleTimeoutMinutes as IdleTimeoutMinutes
        ,@idleExpires as IdleExpires
        ,@absoluteExpires as AbsoluteExpires