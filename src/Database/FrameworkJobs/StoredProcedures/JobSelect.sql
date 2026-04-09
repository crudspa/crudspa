create proc [FrameworkJobs].[JobSelect] (
     @Id uniqueidentifier
) as

select
     job.Id
    ,job.TypeId
    ,job.Config
    ,job.Description
    ,job.Added
    ,job.Started
    ,job.Ended
    ,job.StatusId
    ,job.DeviceId
    ,job.ScheduleId
    ,job.BatchId
     ,type.Name as TypeName
    ,status.Name as StatusName
    ,status.CssClass as StatusCssClass
    ,device.Name as DeviceName
    ,schedule.Name as ScheduleName
from [Framework].[Job-Active] job
    inner join [Framework].[JobType-Active] type on job.TypeId = type.Id
    inner join [Framework].[JobStatus-Active] status on job.StatusId = status.Id
    left join [Framework].[Device-Active] device on job.DeviceId = device.Id
    left join [Framework].[JobSchedule-Active] schedule on job.ScheduleId = schedule.Id
where job.Id = @Id