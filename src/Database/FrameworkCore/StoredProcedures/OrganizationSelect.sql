create proc [FrameworkCore].[OrganizationSelect] (
     @Id uniqueidentifier
    ,@PortalId uniqueidentifier
) as

select
     organization.Id
    ,organization.Name
    ,organization.TimeZoneId
from [Framework].[Organization-Active] organization
where Id = @Id

select
     role.Id
    ,role.Name
    ,role.OrganizationId
from [Framework].[Role-Active] role
where role.OrganizationId = @Id

select
     permission.Id
    ,permission.Name
from [Framework].[Permission-Active] permission
    inner join [Framework].[PortalPermission-Active] portalPermission on portalPermission.PermissionId = permission.Id
where portalPermission.PortalId = @PortalId
order by permission.Name

select distinct
     rolePermission.RoleId
    ,rolePermission.PermissionId
from [Framework].[RolePermission-Active] rolePermission
    inner join [Framework].[Role-Active] role on rolePermission.RoleId = role.Id
where role.OrganizationId = @Id