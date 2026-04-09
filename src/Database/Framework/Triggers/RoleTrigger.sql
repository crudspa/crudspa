create trigger [Framework].[RoleTrigger] on [Framework].[Role]
    for update
as

insert [Framework].[Role] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Name
    ,OrganizationId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Name
    ,deleted.OrganizationId
from deleted