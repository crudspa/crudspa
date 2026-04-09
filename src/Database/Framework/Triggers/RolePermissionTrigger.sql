create trigger [Framework].[RolePermissionTrigger] on [Framework].[RolePermission]
    for update
as

insert [Framework].[RolePermission] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,RoleId
    ,PermissionId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.RoleId
    ,deleted.PermissionId
from deleted