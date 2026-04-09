create view [Framework].[RolePermission-Active] as

select rolePermission.Id as Id
    ,rolePermission.RoleId as RoleId
    ,rolePermission.PermissionId as PermissionId
from [Framework].[RolePermission] rolePermission
where 1=1
    and rolePermission.IsDeleted = 0
    and rolePermission.VersionOf = rolePermission.Id