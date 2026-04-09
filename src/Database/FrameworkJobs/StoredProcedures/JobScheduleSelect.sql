create proc [FrameworkJobs].[JobScheduleSelect] (
     @Id uniqueidentifier
) as

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
where jobSchedule.Id = @Id