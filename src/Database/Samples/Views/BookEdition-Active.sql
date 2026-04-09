create view [Samples].[BookEdition-Active] as

select bookEdition.Id as Id
    ,bookEdition.BookId as BookId
    ,bookEdition.FormatId as FormatId
    ,bookEdition.Sku as Sku
    ,bookEdition.Price as Price
    ,bookEdition.ReleasedOn as ReleasedOn
    ,bookEdition.InPrint as InPrint
    ,bookEdition.Ordinal as Ordinal
from [Samples].[BookEdition] bookEdition
where 1=1
    and bookEdition.IsDeleted = 0
    and bookEdition.VersionOf = bookEdition.Id