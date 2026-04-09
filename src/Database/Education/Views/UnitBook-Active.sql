create view [Education].[UnitBook-Active] as

select unitBook.Id as Id
    ,unitBook.UnitId as UnitId
    ,unitBook.BookId as BookId
    ,unitBook.Treatment as Treatment
    ,unitBook.Control as Control
    ,unitBook.Ordinal as Ordinal
from [Education].[UnitBook] unitBook
where 1=1
    and unitBook.IsDeleted = 0
    and unitBook.VersionOf = unitBook.Id