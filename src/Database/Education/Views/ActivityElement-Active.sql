create view [Education].[ActivityElement-Active] as

select activityElement.Id as Id
    ,activityElement.ElementId as ElementId
    ,activityElement.ActivityId as ActivityId
from [Education].[ActivityElement] activityElement
where 1=1
    and activityElement.IsDeleted = 0
    and activityElement.VersionOf = activityElement.Id