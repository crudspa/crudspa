create trigger [Samples].[ShirtOptionTrigger] on [Samples].[ShirtOption]
    for update
as

insert [Samples].[ShirtOption] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ShirtId
    ,ColorId
    ,SkuBase
    ,Price
    ,AllSizes
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ShirtId
    ,deleted.ColorId
    ,deleted.SkuBase
    ,deleted.Price
    ,deleted.AllSizes
    ,deleted.Ordinal
from deleted