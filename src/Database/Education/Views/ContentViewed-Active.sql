create view [Education].[ContentViewed-Active] as

select contentViewed.Id as Id
    ,contentViewed.BookId as BookId
from [Education].[ContentViewed] contentViewed
where 1=1
    and contentViewed.IsDeleted = 0