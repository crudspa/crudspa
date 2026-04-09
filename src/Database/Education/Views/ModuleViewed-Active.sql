create view [Education].[ModuleViewed-Active] as

select moduleViewed.Id as Id
    ,moduleViewed.ModuleId as ModuleId
from [Education].[ModuleViewed] moduleViewed
where 1=1
    and moduleViewed.IsDeleted = 0