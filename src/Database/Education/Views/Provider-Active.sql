create view [Education].[Provider-Active] as

select provider.Id as Id
    ,provider.OrganizationId as OrganizationId
from [Education].[Provider] provider
where 1=1
    and provider.IsDeleted = 0
    and provider.VersionOf = provider.Id