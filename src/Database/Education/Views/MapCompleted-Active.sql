create view [Education].[MapCompleted-Active] as

select mapCompleted.Id as Id
    ,mapCompleted.StudentId as StudentId
    ,mapCompleted.BookId as BookId
    ,mapCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[MapCompleted] mapCompleted
where 1=1
    and mapCompleted.IsDeleted = 0