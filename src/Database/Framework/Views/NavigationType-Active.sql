create view [Framework].[NavigationType-Active] as

select navigationType.Id as Id
    ,navigationType.Name as Name
    ,navigationType.DisplayView as DisplayView
from [Framework].[NavigationType] navigationType
where 1=1
    and navigationType.IsDeleted = 0