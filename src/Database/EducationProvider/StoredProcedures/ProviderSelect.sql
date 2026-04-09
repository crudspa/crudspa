create proc [EducationProvider].[ProviderSelect] (
     @SessionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     provider.Id
    ,provider.OrganizationId
from [Education].[Provider-Active] provider
    inner join [Framework].[Organization-Active] organization on provider.OrganizationId = organization.Id
where 1 = 1
    and organization.Id = @organizationId