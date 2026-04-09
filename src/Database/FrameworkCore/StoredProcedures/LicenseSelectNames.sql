create proc [FrameworkCore].[LicenseSelectNames] (
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
     license.Id
    ,license.Name as Name
from [Framework].[License-Active] license
where license.OwnerId = @organizationId
order by license.Name