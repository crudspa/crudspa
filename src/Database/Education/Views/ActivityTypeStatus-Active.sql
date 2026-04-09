create view [Education].[ActivityTypeStatus-Active] as

select activityTypeStatus.Id as Id
    ,activityTypeStatus.Name as Name
    ,activityTypeStatus.Ordinal as Ordinal
from [Education].[ActivityTypeStatus] activityTypeStatus
where 1=1
    and activityTypeStatus.IsDeleted = 0