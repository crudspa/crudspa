create view [Education].[BookViewed-Active] as

select bookViewed.Id as Id
    ,bookViewed.BookId as BookId
from [Education].[BookViewed] bookViewed
where 1=1
    and bookViewed.IsDeleted = 0