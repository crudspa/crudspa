create view [Framework].[Job-Active] as

select job.Id as Id
    ,job.TypeId as TypeId
    ,job.Config as Config
    ,job.Description as Description
    ,job.Added as Added
    ,job.Started as Started
    ,job.Ended as Ended
    ,job.StatusId as StatusId
    ,job.DeviceId as DeviceId
    ,job.ScheduleId as ScheduleId
    ,job.BatchId as BatchId
from [Framework].[Job] job
where 1=1
    and job.IsDeleted = 0