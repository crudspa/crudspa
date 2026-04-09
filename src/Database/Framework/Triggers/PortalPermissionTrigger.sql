create trigger [Framework].[PortalPermissionTrigger] on [Framework].[PortalPermission]
    for update
as

insert [Framework].[PortalPermission] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,PermissionId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PortalId
    ,deleted.PermissionId
from deleted