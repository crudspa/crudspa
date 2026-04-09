create view [Education].[Chapter-Active] as

select chapter.Id as Id
    ,chapter.Title as Title
    ,chapter.BookId as BookId
    ,chapter.BinderId as BinderId
    ,chapter.Ordinal as Ordinal
from [Education].[Chapter] chapter
where 1=1
    and chapter.IsDeleted = 0
    and chapter.VersionOf = chapter.Id