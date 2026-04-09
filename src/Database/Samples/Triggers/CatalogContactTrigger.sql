create trigger [Samples].[CatalogContactTrigger] on [Samples].[CatalogContact]
    for update
as

insert [Samples].[CatalogContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ContactId
    ,UserId
    ,LastIp
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ContactId
    ,deleted.UserId
    ,deleted.LastIp
from deleted