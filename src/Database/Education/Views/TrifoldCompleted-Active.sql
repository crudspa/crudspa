create view [Education].[TrifoldCompleted-Active] as

select trifoldCompleted.Id as Id
    ,trifoldCompleted.StudentId as StudentId
    ,trifoldCompleted.TrifoldId as TrifoldId
    ,trifoldCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[TrifoldCompleted] trifoldCompleted
where 1=1
    and trifoldCompleted.IsDeleted = 0