create proc [FrameworkJobs].[JobScheduleInsert] (
     @SessionId uniqueidentifier
    ,@Name nvarchar(50)
    ,@TypeId uniqueidentifier
    ,@Config nvarchar(max)
    ,@Description nvarchar(max)
    ,@DeviceId uniqueidentifier
    ,@RecurrenceAmount int
    ,@RecurrenceInterval int
    ,@RecurrencePattern int
    ,@Day int
    ,@DayOfWeek int
    ,@Hour int
    ,@Minute int
    ,@Second int
    ,@TimeZoneId nvarchar(32)
    ,@NextRun datetimeoffset(7)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[JobSchedule] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
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
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Name
    ,@TypeId
    ,@Config
    ,@Description
    ,@DeviceId
    ,@RecurrenceAmount
    ,@RecurrenceInterval
    ,@RecurrencePattern
    ,@Day
    ,@DayOfWeek
    ,@Hour
    ,@Minute
    ,@Second
    ,@TimeZoneId
    ,@NextRun
)