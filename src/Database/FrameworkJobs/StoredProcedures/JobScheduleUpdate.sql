create proc [FrameworkJobs].[JobScheduleUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[JobSchedule]
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Name = @Name
    ,TypeId = @TypeId
    ,Config = @Config
    ,Description = @Description
    ,DeviceId = @DeviceId
    ,RecurrenceAmount = @RecurrenceAmount
    ,RecurrenceInterval = @RecurrenceInterval
    ,RecurrencePattern = @RecurrencePattern
    ,Day = @Day
    ,DayOfWeek = @DayOfWeek
    ,Hour = @Hour
    ,Minute = @Minute
    ,Second = @Second
    ,TimeZoneId = @TimeZoneId
    ,NextRun = @NextRun
where Id = @Id