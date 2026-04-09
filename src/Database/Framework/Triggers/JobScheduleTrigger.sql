create trigger [Framework].[JobScheduleTrigger] on [Framework].[JobSchedule]
    for update
as

insert [Framework].[JobSchedule] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Name
    ,TypeId
    ,Config
    ,Description
    ,DeviceId
    ,RecurrenceAmount
    ,RecurrenceInterval
    ,RecurrencePattern
    ,Day
    ,DayOfWeek
    ,Hour
    ,Minute
    ,Second
    ,TimeZoneId
    ,NextRun
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Name
    ,deleted.TypeId
    ,deleted.Config
    ,deleted.Description
    ,deleted.DeviceId
    ,deleted.RecurrenceAmount
    ,deleted.RecurrenceInterval
    ,deleted.RecurrencePattern
    ,deleted.Day
    ,deleted.DayOfWeek
    ,deleted.Hour
    ,deleted.Minute
    ,deleted.Second
    ,deleted.TimeZoneId
    ,deleted.NextRun
from deleted