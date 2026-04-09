create view [Education].[ListenPartCompleted-Active] as

select listenPartCompleted.Id as Id
    ,listenPartCompleted.AssignmentId as AssignmentId
    ,listenPartCompleted.ListenPartId as ListenPartId
    ,listenPartCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[ListenPartCompleted] listenPartCompleted
where 1=1
    and listenPartCompleted.IsDeleted = 0