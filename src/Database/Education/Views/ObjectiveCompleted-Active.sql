create view [Education].[ObjectiveCompleted-Active] as

select objectiveCompleted.Id as Id
    ,objectiveCompleted.StudentId as StudentId
    ,objectiveCompleted.ObjectiveId as ObjectiveId
    ,objectiveCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[ObjectiveCompleted] objectiveCompleted
where 1=1
    and objectiveCompleted.IsDeleted = 0