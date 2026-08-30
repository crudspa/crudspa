create proc [FrameworkAuth].[AuthStartPolicySelect] (
     @Provider nvarchar(75)
    ,@Audience nvarchar(25)
    ,@Tenant nvarchar(255)
) as

set nocount on

select
     connection.Provider
    ,connection.Tenant
    ,policy.AutoRedirect
    ,policy.Fallback
from [Framework].[AuthPolicy-Active] policy
    inner join [Framework].[AuthConnection-Active] connection on connection.Id = policy.AuthConnectionId
        and connection.OrganizationId = policy.OrganizationId
        and connection.Enabled = 1
where policy.Audience collate Latin1_General_100_BIN2 = @Audience collate Latin1_General_100_BIN2
    and policy.Enabled = 1
    and connection.Tenant collate Latin1_General_100_BIN2 = @Tenant collate Latin1_General_100_BIN2
    and (@Provider is null or connection.Provider collate Latin1_General_100_BIN2 = @Provider collate Latin1_General_100_BIN2)