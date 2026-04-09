create view [Education].[MapViewed-Active] as

select mapViewed.Id as Id
    ,mapViewed.BookId as BookId
from [Education].[MapViewed] mapViewed
where 1=1
    and mapViewed.IsDeleted = 0