create view [Education].[PrefaceCompleted-Active] as

select prefaceCompleted.Id as Id
    ,prefaceCompleted.StudentId as StudentId
    ,prefaceCompleted.BookId as BookId
    ,prefaceCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[PrefaceCompleted] prefaceCompleted
where 1=1
    and prefaceCompleted.IsDeleted = 0