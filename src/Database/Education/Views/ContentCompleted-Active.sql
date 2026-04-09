create view [Education].[ContentCompleted-Active] as

select contentCompleted.Id as Id
    ,contentCompleted.StudentId as StudentId
    ,contentCompleted.BookId as BookId
    ,contentCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[ContentCompleted] contentCompleted
where 1=1
    and contentCompleted.IsDeleted = 0