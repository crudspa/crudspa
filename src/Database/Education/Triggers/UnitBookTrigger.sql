create trigger [Education].[UnitBookTrigger] on [Education].[UnitBook]
    for update
as

insert [Education].[UnitBook] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,UnitId
    ,BookId
    ,Treatment
    ,Control
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.UnitId
    ,deleted.BookId
    ,deleted.Treatment
    ,deleted.Control
    ,deleted.Ordinal
from deleted