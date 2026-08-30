create view [Framework].[AuthConnection-Active] as

select authConnection.Id as Id
    ,authConnection.OrganizationId as OrganizationId
    ,authConnection.Provider as Provider
    ,authConnection.Tenant as Tenant
    ,authConnection.Enabled as Enabled
from [Framework].[AuthConnection] authConnection
where 1=1
    and authConnection.IsDeleted = 0
    and authConnection.VersionOf = authConnection.Id