create trigger [Samples].[BookEditionTrigger] on [Samples].[BookEdition]
    for update
as

insert [Samples].[BookEdition] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BookId
    ,FormatId
    ,Sku
    ,Price
    ,ReleasedOn
    ,InPrint
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BookId
    ,deleted.FormatId
    ,deleted.Sku
    ,deleted.Price
    ,deleted.ReleasedOn
    ,deleted.InPrint
    ,deleted.Ordinal
from deleted