create proc [EducationCommon].[ReportSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
)
as

declare @sessionPortalId uniqueidentifier
declare @sessionUserId uniqueidentifier

select top 1
     @sessionPortalId = session.PortalId
    ,@sessionUserId = session.UserId
from [Framework].[Session-Active] session
where session.Id = @SessionId

;with sessionPermission as (
    select distinct rolePermission.PermissionId
    from [Framework].[User-Active] userTable
        inner join [Framework].[UserRole-Active] userRole on userRole.UserId = userTable.Id
        inner join [Framework].[RolePermission-Active] rolePermission on rolePermission.RoleId = userRole.RoleId
        inner join [Framework].[PortalPermission-Active] portalPermission on portalPermission.PermissionId = rolePermission.PermissionId
        inner join [Framework].[Permission-Active] permission on permission.Id = rolePermission.PermissionId
    where userTable.Id = @sessionUserId
        and portalPermission.PortalId = @sessionPortalId
)
select top 1
     report.Id
    ,report.IconId
    ,report.DisplayView
    ,report.Name
    ,report.Description
    ,report.Ordinal
    ,icon.CssClass as IconCssClass
from [Education].[Report-Active] report
    inner join [Framework].[Icon-Active] icon on report.IconId = icon.Id
    left join sessionPermission permission on permission.PermissionId = report.PermissionId
where report.Id = @Id
    and report.PortalId = @sessionPortalId
    and (report.PermissionId is null or permission.PermissionId is not null)