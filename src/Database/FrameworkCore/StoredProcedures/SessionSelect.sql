create proc [FrameworkCore].[SessionSelect] (
     @Id uniqueidentifier
    ,@PortalId uniqueidentifier
) as

set nocount on;

select
     session.Id
    ,session.PortalId
    ,userTable.Id as UserId
    ,userTable.Username as UserUsername
    ,userTable.ResetPassword as UserResetPassword
    ,userTable.OrganizationId as UserOrganizationId
    ,contact.Id as ContactId
    ,contact.FirstName as ContactFirstName
    ,contact.LastName as ContactLastName
    ,contact.TimeZoneId as ContactTimeZoneId
from [Framework].[Session-Active] session
    left join [Framework].[User-Active] userTable on session.UserId = userTable.Id
    left join [Framework].[Contact-Active] contact on userTable.ContactId = contact.Id
where session.Id = @Id
    and session.PortalId = @PortalId
    and session.Ended is null

select distinct permission.Id
from [Framework].[Session-Active] session
    inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    inner join [Framework].[UserRole-Active] userRole on userRole.UserId = userTable.Id
    inner join [Framework].[RolePermission-Active] rolePermission on rolePermission.RoleId = userRole.RoleId
    inner join [Framework].[PortalPermission-Active] portalPermission on portalPermission.PortalId = @PortalId
        and portalPermission.PermissionId = rolePermission.PermissionId
    inner join [Framework].[Permission-Active] permission on permission.Id = rolePermission.PermissionId
where session.Id = @Id
    and session.PortalId = @PortalId
    and session.Ended is null