create proc [FrameworkAuth].[AuthHandoffRedeem] (
     @CodeHash binary(32)
    ,@PortalId uniqueidentifier
    ,@SessionId uniqueidentifier
    ,@PreviousSessionId uniqueidentifier
) as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()
declare @id uniqueidentifier
declare @transactionId uniqueidentifier
declare @authPolicyId uniqueidentifier
declare @externalIdentityId uniqueidentifier
declare @provider nvarchar(75)
declare @tenant nvarchar(255)
declare @audience nvarchar(25)
declare @returnPath nvarchar(500)
declare @userId uniqueidentifier
declare @idleTimeoutMinutes int
declare @absoluteTimeoutMinutes int
declare @persist bit
declare @idleExpires datetimeoffset
declare @absoluteExpires datetimeoffset

begin transaction

select
     @id = handoff.Id
    ,@transactionId = handoff.AuthTransactionId
    ,@authPolicyId = handoff.AuthPolicyId
    ,@externalIdentityId = handoff.ExternalIdentityId
    ,@provider = externalIdentity.Provider
    ,@tenant = externalIdentity.Tenant
    ,@audience = portal.[Key]
    ,@returnPath = tx.ReturnPath
    ,@userId = handoff.UserId
    ,@idleTimeoutMinutes = policy.IdleTimeoutMinutes
    ,@absoluteTimeoutMinutes = policy.AbsoluteTimeoutMinutes
    ,@persist = policy.Persist
from [Framework].[AuthHandoff] handoff with (updlock, holdlock)
    inner join [Framework].[AuthTransaction] tx on handoff.AuthTransactionId = tx.Id
    inner join [Framework].[ExternalIdentity-Active] externalIdentity on handoff.ExternalIdentityId = externalIdentity.Id
        and externalIdentity.Enabled = 1
    inner join [Framework].[ExternalIdentityLink-Active] link on handoff.ExternalIdentityId = link.ExternalIdentityId
        and handoff.UserId = link.UserId
    inner join [Framework].[User-Active] [user] on handoff.UserId = [user].Id
        and handoff.PortalId = [user].PortalId
    inner join [Framework].[Portal-Active] portal on handoff.PortalId = portal.Id
    inner join [Framework].[AuthPolicy-Active] policy on handoff.AuthPolicyId = policy.Id
        and policy.Audience collate Latin1_General_100_BIN2 = portal.[Key] collate Latin1_General_100_BIN2
        and policy.Enabled = 1
    inner join [Framework].[AuthConnection-Active] connection on policy.AuthConnectionId = connection.Id
        and policy.OrganizationId = connection.OrganizationId
        and connection.Provider collate Latin1_General_100_BIN2 = externalIdentity.Provider collate Latin1_General_100_BIN2
        and connection.Tenant collate Latin1_General_100_BIN2 = externalIdentity.Tenant collate Latin1_General_100_BIN2
        and connection.Enabled = 1
where handoff.CodeHash = @CodeHash
    and handoff.PortalId = @PortalId
    and handoff.Consumed is null
    and handoff.Expires > @now

if @id is not null
    and (@idleTimeoutMinutes <= 0 or @absoluteTimeoutMinutes < @idleTimeoutMinutes)
    set @id = null

if @id is not null
begin
    set @absoluteExpires = dateadd(minute, @absoluteTimeoutMinutes, @now)
    set @idleExpires = dateadd(minute, @idleTimeoutMinutes, @now)

    update [Framework].[AuthHandoff]
    set Consumed = @now
    where Id = @id

    if @PreviousSessionId is not null
        and @PreviousSessionId <> @SessionId
        and exists (
            select 1
            from [Framework].[SessionAuth] sessionAuth
                inner join [Framework].[Session-Active] session on sessionAuth.SessionId = session.Id
            where sessionAuth.SessionId = @PreviousSessionId
                and session.PortalId = @PortalId
        )
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
            ,@PreviousSessionId
            ,N'session-revoked'
            ,N'succeeded'
            ,sessionAuth.Provider
            ,externalIdentity.Tenant
            ,@audience
            ,@PortalId
            ,sessionAuth.ExternalIdentityId
            ,@PreviousSessionId
            ,N'rotated'
        from [Framework].[SessionAuth] sessionAuth
            left join [Framework].[ExternalIdentity] externalIdentity on sessionAuth.ExternalIdentityId = externalIdentity.Id
        where sessionAuth.SessionId = @PreviousSessionId

        update [Framework].[SessionAuth]
        set Revoked = coalesce(Revoked, @now)
            ,RevocationReason = coalesce(RevocationReason, N'rotated')
        where SessionId = @PreviousSessionId

        exec [FrameworkCore].[SessionEnd] @PreviousSessionId
    end

    exec [FrameworkCore].[SessionInsert] @SessionId, @PortalId
    exec [FrameworkCore].[SessionUpdateUser] @SessionId, @userId

    insert [Framework].[SessionAuth] (
         Id
        ,SessionId
        ,ExternalIdentityId
        ,Provider
        ,Authenticated
        ,LastActivity
        ,IdleTimeoutMinutes
        ,IdleExpires
        ,AbsoluteExpires
        ,AuthPolicyId
    )
    values (
         newid()
        ,@SessionId
        ,@externalIdentityId
        ,@provider
        ,@now
        ,@now
        ,@idleTimeoutMinutes
        ,@idleExpires
        ,@absoluteExpires
        ,@authPolicyId
    )

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
    )
    values (
         newid()
        ,@now
        ,@transactionId
        ,N'session-started'
        ,N'succeeded'
        ,@provider
        ,@tenant
        ,@audience
        ,@PortalId
        ,@externalIdentityId
        ,@SessionId
    )

    select
         @userId as UserId
        ,@externalIdentityId as ExternalIdentityId
        ,@sessionId as SessionId
        ,@authPolicyId as AuthPolicyId
        ,@absoluteExpires as AbsoluteExpires
        ,@returnPath as ReturnPath
        ,@persist as Persist
end

commit transaction