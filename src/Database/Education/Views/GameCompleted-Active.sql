create view [Education].[GameCompleted-Active] as

select gameCompleted.Id as Id
    ,gameCompleted.StudentId as StudentId
    ,gameCompleted.BookId as BookId
    ,gameCompleted.GameId as GameId
    ,gameCompleted.GameRunId as GameRunId
    ,gameCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[GameCompleted] gameCompleted
where 1=1
    and gameCompleted.IsDeleted = 0