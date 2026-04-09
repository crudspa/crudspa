create view [Framework].[UserRole-Active] as

select userRole.Id as Id
    ,userRole.UserId as UserId
    ,userRole.RoleId as RoleId
from [Framework].[UserRole] userRole
where 1=1
    and userRole.IsDeleted = 0
    and userRole.VersionOf = userRole.Id