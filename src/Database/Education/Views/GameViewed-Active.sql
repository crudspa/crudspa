create view [Education].[GameViewed-Active] as

select gameViewed.Id as Id
    ,gameViewed.GameId as GameId
from [Education].[GameViewed] gameViewed
where 1=1
    and gameViewed.IsDeleted = 0