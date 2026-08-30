create view [Framework].[SessionAuth-Active] as

select sessionAuth.Id as Id
    ,sessionAuth.SessionId as SessionId
    ,sessionAuth.ExternalIdentityId as ExternalIdentityId
    ,sessionAuth.Provider as Provider
    ,sessionAuth.Authenticated as Authenticated
    ,sessionAuth.LastActivity as LastActivity
    ,sessionAuth.IdleTimeoutMinutes as IdleTimeoutMinutes
    ,sessionAuth.IdleExpires as IdleExpires
    ,sessionAuth.AbsoluteExpires as AbsoluteExpires
    ,sessionAuth.Revoked as Revoked
    ,sessionAuth.RevocationReason as RevocationReason
    ,sessionAuth.AuthPolicyId as AuthPolicyId
from [Framework].[SessionAuth] sessionAuth
where sessionAuth.Revoked is null