create trigger [Samples].[ShirtOptionSizeTrigger] on [Samples].[ShirtOptionSize]
    for update
as

insert [Samples].[ShirtOptionSize] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ShirtOptionId
    ,SizeId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ShirtOptionId
    ,deleted.SizeId
from deleted