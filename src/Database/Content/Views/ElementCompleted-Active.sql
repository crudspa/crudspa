create view [Content].[ElementCompleted-Active] as

select elementCompleted.Id as Id
    ,elementCompleted.ContactId as ContactId
    ,elementCompleted.ElementId as ElementId
    ,elementCompleted.DeviceTimestamp as DeviceTimestamp
from [Content].[ElementCompleted] elementCompleted
where 1=1
    and elementCompleted.IsDeleted = 0