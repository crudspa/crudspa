create trigger [Framework].[AuthConnectionTrigger] on [Framework].[AuthConnection]
    for update
as

insert [Framework].[AuthConnection] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OrganizationId
    ,Provider
    ,Tenant
    ,Enabled
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OrganizationId
    ,deleted.Provider
    ,deleted.Tenant
    ,deleted.Enabled
from deleted