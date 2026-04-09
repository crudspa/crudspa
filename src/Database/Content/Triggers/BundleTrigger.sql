create trigger [Content].[BundleTrigger] on [Content].[Bundle]
    for update
as

insert [Content].[Bundle] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Name
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Name
from deleted