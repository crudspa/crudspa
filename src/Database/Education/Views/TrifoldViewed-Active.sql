create view [Education].[TrifoldViewed-Active] as

select trifoldViewed.Id as Id
    ,trifoldViewed.TrifoldId as TrifoldId
from [Education].[TrifoldViewed] trifoldViewed
where 1=1
    and trifoldViewed.IsDeleted = 0