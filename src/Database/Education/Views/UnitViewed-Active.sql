create view [Education].[UnitViewed-Active] as

select unitViewed.Id as Id
    ,unitViewed.UnitId as UnitId
from [Education].[UnitViewed] unitViewed
where 1=1
    and unitViewed.IsDeleted = 0