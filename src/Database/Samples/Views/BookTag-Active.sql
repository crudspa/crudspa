create view [Samples].[BookTag-Active] as

select bookTag.Id as Id
    ,bookTag.BookId as BookId
    ,bookTag.TagId as TagId
from [Samples].[BookTag] bookTag
where 1=1
    and bookTag.IsDeleted = 0
    and bookTag.VersionOf = bookTag.Id