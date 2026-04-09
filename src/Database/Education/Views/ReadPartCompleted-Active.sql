create view [Education].[ReadPartCompleted-Active] as

select readPartCompleted.Id as Id
    ,readPartCompleted.AssignmentId as AssignmentId
    ,readPartCompleted.ReadPartId as ReadPartId
    ,readPartCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[ReadPartCompleted] readPartCompleted
where 1=1
    and readPartCompleted.IsDeleted = 0