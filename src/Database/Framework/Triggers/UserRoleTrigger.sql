create trigger [Framework].[UserRoleTrigger] on [Framework].[UserRole]
    for update
as

insert [Framework].[UserRole] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,UserId
    ,RoleId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.UserId
    ,deleted.RoleId
from deleted