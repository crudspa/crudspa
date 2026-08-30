create view [Framework].[AuthHandoff-Active] as

select handoff.Id as Id
    ,handoff.AuthTransactionId as AuthTransactionId
    ,handoff.CodeHash as CodeHash
    ,handoff.PortalId as PortalId
    ,handoff.UserId as UserId
    ,handoff.ExternalIdentityId as ExternalIdentityId
    ,handoff.Created as Created
    ,handoff.Expires as Expires
    ,handoff.Consumed as Consumed
    ,handoff.AuthPolicyId as AuthPolicyId
from [Framework].[AuthHandoff] handoff
where 1=1