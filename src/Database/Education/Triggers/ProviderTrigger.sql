create trigger [Education].[ProviderTrigger] on [Education].[Provider]
    for update
as

insert [Education].[Provider] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OrganizationId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OrganizationId
from deleted