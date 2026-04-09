create proc [FrameworkJobs].[JobScheduleSelectAll] as

set nocount on

declare @JobStatusCompleted uniqueidentifier = '81c1ccdb-cbf3-4a6a-845e-ca8839c17d2d'
declare @JobStatusFailed uniqueidentifier = 'c6416f41-8dc5-424d-a53b-04f13ad3568d'

select
     jobSchedule.Id
    ,jobSchedule.Name
    ,jobSchedule.TypeId
    ,jobSchedule.Config
    ,jobSchedule.Description
    ,jobSchedule.DeviceId
    ,jobSchedule.RecurrenceAmount
    ,jobSchedule.RecurrenceInterval
    ,jobSchedule.RecurrencePattern
    ,jobSchedule.Day
    ,jobSchedule.DayOfWeek
    ,jobSchedule.Hour
    ,jobSchedule.Minute
    ,jobSchedule.Second
    ,jobSchedule.TimeZoneId
    ,jobSchedule.NextRun
     ,lastJob.LastRun
    ,jobStatus.Name as LastStatus
    ,jobStatus.CssClass as LastStatusCssClass
    ,type.Name as TypeName
    ,device.Name as DeviceName
from [Framework].[JobSchedule-Active] jobSchedule
    inner join [Framework].[JobType-Active] type on jobSchedule.TypeId = type.Id
    left join [Framework].[Device-Active] device on jobSchedule.DeviceId = device.Id
    outer apply (
        select top (1)
            job.Ended as LastRun
            ,job.StatusId
        from [Framework].[Job-Active] job
        where job.ScheduleId = jobSchedule.Id
          and job.StatusId in (@JobStatusCompleted, @JobStatusFailed)
        order by job.Ended desc
    ) lastJob
    left join [Framework].[JobStatus-Active] jobStatus on lastJob.StatusId = jobStatus.Id