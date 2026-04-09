create view [Education].[ModuleCompleted-Active] as

select moduleCompleted.Id as Id
    ,moduleCompleted.StudentId as StudentId
    ,moduleCompleted.ModuleId as ModuleId
    ,moduleCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[ModuleCompleted] moduleCompleted
where 1=1
    and moduleCompleted.IsDeleted = 0