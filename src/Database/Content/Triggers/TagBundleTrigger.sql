create trigger [Content].[TagBundleTrigger] on [Content].[TagBundle]
    for update
as

insert [Content].[TagBundle] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,TagId
    ,BundleId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.TagId
    ,deleted.BundleId
from deleted