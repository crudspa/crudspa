create view [Framework].[AuthEvent-Active] as

select authenticationEvent.Id as Id
    ,authenticationEvent.Created as Created
    ,authenticationEvent.CorrelationId as CorrelationId
    ,authenticationEvent.Type as Type
    ,authenticationEvent.Outcome as Outcome
    ,authenticationEvent.Provider as Provider
    ,authenticationEvent.Tenant as Tenant
    ,authenticationEvent.Audience as Audience
    ,authenticationEvent.PortalId as PortalId
    ,authenticationEvent.ExternalIdentityId as ExternalIdentityId
    ,authenticationEvent.SessionId as SessionId
    ,authenticationEvent.Reason as Reason
from [Framework].[AuthEvent] authenticationEvent
where 1=1