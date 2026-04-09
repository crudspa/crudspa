create view [Education].[ActivityStatus-Active] as

select activityStatus.Id as Id
    ,activityStatus.Name as Name
    ,activityStatus.Ordinal as Ordinal
from [Education].[ActivityStatus] activityStatus
where 1=1
    and activityStatus.IsDeleted = 0