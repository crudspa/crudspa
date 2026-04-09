create view [Framework].[JobSchedule-Active] as

select jobSchedule.Id as Id
    ,jobSchedule.Name as Name
    ,jobSchedule.TypeId as TypeId
    ,jobSchedule.Config as Config
    ,jobSchedule.Description as Description
    ,jobSchedule.DeviceId as DeviceId
    ,jobSchedule.RecurrenceAmount as RecurrenceAmount
    ,jobSchedule.RecurrenceInterval as RecurrenceInterval
    ,jobSchedule.RecurrencePattern as RecurrencePattern
    ,jobSchedule.Day as Day
    ,jobSchedule.DayOfWeek as DayOfWeek
    ,jobSchedule.Hour as Hour
    ,jobSchedule.Minute as Minute
    ,jobSchedule.Second as Second
    ,jobSchedule.TimeZoneId as TimeZoneId
    ,jobSchedule.NextRun as NextRun
from [Framework].[JobSchedule] jobSchedule
where 1=1
    and jobSchedule.IsDeleted = 0
    and jobSchedule.VersionOf = jobSchedule.Id