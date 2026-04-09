create trigger [Samples].[CatalogTrigger] on [Samples].[Catalog]
    for update
as

insert [Samples].[Catalog] (
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