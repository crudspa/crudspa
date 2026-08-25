create proc [ContentDesign].[ForumPermissionSelectNames] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select distinct
     permission.Id
    ,permission.Name
from [Framework].[Permission-Active] permission
    inner join [Framework].[PortalPermission-Active] portalPermission on portalPermission.PermissionId = permission.Id
    inner join [Framework].[Portal-Active] portal on portal.Id = portalPermission.PortalId
where portal.Id = @PortalId
    and portal.OwnerId = @organizationId
order by permission.Name