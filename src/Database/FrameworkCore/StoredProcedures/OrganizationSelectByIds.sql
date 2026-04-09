create proc [FrameworkCore].[OrganizationSelectByIds] (
     @Ids Framework.IdList readonly
    ,@PortalId uniqueidentifier
) as

set nocount on

select
     organization.Id
    ,organization.Name
    ,organization.TimeZoneId
from [Framework].[Organization-Active] organization
    inner join @Ids ids on ids.Id = organization.Id

select
     role.Id
    ,role.Name
    ,role.OrganizationId
from [Framework].[Role-Active] role
    inner join @Ids ids on ids.Id = role.OrganizationId

select
     permission.Id
    ,permission.Name
from [Framework].[Permission-Active] permission
    inner join [Framework].[PortalPermission-Active] portalPermission on portalPermission.PermissionId = permission.Id
where portalPermission.PortalId = @PortalId

select distinct
     rolePermission.RoleId
    ,rolePermission.PermissionId
from [Framework].[RolePermission-Active] rolePermission
    inner join [Framework].[Role-Active] role on rolePermission.RoleId = role.Id
    inner join @Ids ids on ids.Id = role.OrganizationId