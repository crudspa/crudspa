create proc [FrameworkAuth].[AuthPolicySessionsRevoke] (
     @PolicyIds [Framework].[IdList] readonly
    ,@Reason nvarchar(75)
) as

set nocount on

declare @now datetimeoffset = sysdatetimeoffset()

declare @sessions table (
     SessionAuthId uniqueidentifier not null primary key
    ,SessionId uniqueidentifier not null
    ,PortalId uniqueidentifier not null
    ,ExternalIdentityId uniqueidentifier null
    ,Provider nvarchar(75) not null
    ,Tenant nvarchar(255) null
    ,Audience nvarchar(25) not null
)

insert @sessions
select
     sessionAuth.Id
    ,sessionAuth.SessionId
    ,session.PortalId
    ,sessionAuth.ExternalIdentityId
    ,sessionAuth.Provider
    ,authConnection.Tenant
    ,authPolicy.Audience
from [Framework].[SessionAuth] sessionAuth
    inner join @PolicyIds policyId on sessionAuth.AuthPolicyId = policyId.Id
    inner join [Framework].[AuthPolicy] authPolicy on sessionAuth.AuthPolicyId = authPolicy.Id
    inner join [Framework].[AuthConnection] authConnection on authPolicy.AuthConnectionId = authConnection.Id
    inner join [Framework].[Session] session on sessionAuth.SessionId = session.Id
where sessionAuth.Revoked is null

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
    ,session.SessionId
    ,N'session-revoked'
    ,N'succeeded'
    ,session.Provider
    ,session.Tenant
    ,session.Audience
    ,session.PortalId
    ,session.ExternalIdentityId
    ,session.SessionId
    ,@Reason
from @sessions session

update sessionAuth
set Revoked = @now
    ,RevocationReason = @Reason
from [Framework].[SessionAuth] sessionAuth
    inner join @sessions session on sessionAuth.Id = session.SessionAuthId

update session
set IsDeleted = 1
    ,Ended = @now
from [Framework].[Session] session
    inner join @sessions revoked on session.Id = revoked.SessionId
where session.IsDeleted = 0