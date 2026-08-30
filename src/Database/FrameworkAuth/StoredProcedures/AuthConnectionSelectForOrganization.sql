create proc [FrameworkAuth].[AuthConnectionSelectForOrganization] (
     @OrganizationId uniqueidentifier
) as

select
     authConnection.Id
    ,authConnection.OrganizationId
    ,authConnection.Provider
    ,authConnection.Tenant
    ,authConnection.Enabled
from [Framework].[AuthConnection-Active] authConnection
where authConnection.OrganizationId = @OrganizationId
order by authConnection.Provider