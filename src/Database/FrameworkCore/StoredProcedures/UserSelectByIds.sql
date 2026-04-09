create proc [FrameworkCore].[UserSelectByIds] (
     @Ids Framework.IdList readonly
) as

set nocount on

select
     userTable.Id
    ,userTable.Username
    ,userTable.ResetPassword
from [Framework].[User-Active] userTable
    inner join @Ids ids on ids.Id = userTable.Id

select distinct
     userTable.Id as UserId
    ,role.Id as RoleId
    ,role.Name as RoleName
    ,convert(bit, iif(userRole.Id is null, 0, 1)) as Selected
from @Ids ids
    inner join [Framework].[User-Active] userTable on userTable.Id = ids.Id
    inner join [Framework].[Role-Active] role on role.OrganizationId = userTable.OrganizationId
    left join [Framework].[UserRole-Active] userRole on userRole.RoleId = role.Id
        and userRole.UserId = userTable.Id