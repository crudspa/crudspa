create view [Framework].[PortalPermission-Active] as

select portalPermission.Id as Id
    ,portalPermission.PortalId as PortalId
    ,portalPermission.PermissionId as PermissionId
from [Framework].[PortalPermission] portalPermission
where 1=1
    and portalPermission.IsDeleted = 0
    and portalPermission.VersionOf = portalPermission.Id