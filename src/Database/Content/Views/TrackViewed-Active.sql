create view [Content].[TrackViewed-Active] as

select trackViewed.Id as Id
    ,trackViewed.TrackId as TrackId
from [Content].[TrackViewed] trackViewed
where 1=1
    and trackViewed.IsDeleted = 0