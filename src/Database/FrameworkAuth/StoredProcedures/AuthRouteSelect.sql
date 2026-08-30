create proc [FrameworkAuth].[AuthRouteSelect] (
     @Audience nvarchar(25)
    ,@Key nvarchar(75) = null
) as

select
     policy.[Key]
    ,organization.Name
    ,connection.Provider
    ,connection.Tenant
    ,policy.Audience
from [Framework].[AuthPolicy-Active] policy
    inner join [Framework].[AuthConnection-Active] connection
        on policy.AuthConnectionId = connection.Id
        and policy.OrganizationId = connection.OrganizationId
        and connection.Enabled = 1
    inner join [Framework].[Organization-Active] organization on policy.OrganizationId = organization.Id
where policy.Audience = @Audience
    and policy.Enabled = 1
    and policy.[Key] is not null
    and (@Key is null or policy.[Key] = @Key)
order by organization.Name