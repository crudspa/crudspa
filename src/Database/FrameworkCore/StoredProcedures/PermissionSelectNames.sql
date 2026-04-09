create proc [FrameworkCore].[PermissionSelectNames] (
     @PortalId uniqueidentifier
) as

select
     permission.Id
    ,permission.Name
from [Framework].[Permission-Active] permission
    inner join [Framework].[PortalPermission-Active] portalPermission on portalPermission.PermissionId = permission.Id
where portalPermission.PortalId = @PortalId
order by permission.Name