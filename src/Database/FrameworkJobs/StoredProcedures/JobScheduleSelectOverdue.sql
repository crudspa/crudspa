create proc [FrameworkJobs].[JobScheduleSelectOverdue] (
     @SessionId uniqueidentifier
    ,@DeviceId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

declare @JobStatusRunning uniqueidentifier = '28886325-475c-4d3e-9624-96e9c151775d'

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
    ,type.Name as TypeName
    ,device.Name as DeviceName
from [Framework].[JobSchedule-Active] jobSchedule
    inner join [Framework].[JobType-Active] type on jobSchedule.TypeId = type.Id
    left join [Framework].[Device-Active] device on jobSchedule.DeviceId = device.Id
where jobSchedule.NextRun < @now
    and jobSchedule.DeviceId = @DeviceId
    and not exists (
        select 1
        from [Framework].[Job-Active]
        where ScheduleId = jobSchedule.Id
            and StatusId = @JobStatusRunning
    )