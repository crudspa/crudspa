create view [Framework].[ExternalIdentity-Active] as

select externalIdentity.Id as Id
    ,externalIdentity.Created as Created
    ,externalIdentity.Provider as Provider
    ,externalIdentity.Issuer as Issuer
    ,externalIdentity.Subject as Subject
    ,externalIdentity.Tenant as Tenant
    ,externalIdentity.ProviderRole as ProviderRole
    ,externalIdentity.Enabled as Enabled
    ,externalIdentity.LastSeen as LastSeen
    ,externalIdentity.KeyHash as KeyHash
from [Framework].[ExternalIdentity] externalIdentity
where 1=1
    and externalIdentity.IsDeleted = 0