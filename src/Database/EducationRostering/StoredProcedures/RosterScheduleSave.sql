create proc [EducationRostering].[RosterScheduleSave] (
     @SessionId uniqueidentifier
    ,@DeviceId uniqueidentifier
    ,@RosterSourceId uniqueidentifier
    ,@ScheduleHour int
    ,@ScheduleMinute int
    ,@ScheduleTimeZoneId nvarchar(32)
    ,@ScheduleId uniqueidentifier output
) as

set xact_abort on

declare @typeId uniqueidentifier = (
    select Id
    from [Framework].[JobType-Active]
    where Name = N'Roster Sync'
)

if @typeId is null
    throw 50000, 'Roster Sync job type not found.', 1

if not exists (select 1 from [Framework].[Device-Active] where Id = @DeviceId)
    throw 50000, 'Roster jobs device not found.', 1

declare @config nvarchar(max) = concat(N'{"SourceId":"', convert(nvarchar(36), @RosterSourceId), N'","Kind":"full"}')
    ,@description nvarchar(max) = N'Managed daily roster sync'
    ,@name nvarchar(50) = concat(N'Roster ', left(convert(nvarchar(36), @RosterSourceId), 8))
    ,@now datetimeoffset(7) = sysdatetimeoffset()

if @ScheduleId is null or not exists (
    select 1
    from [Framework].[JobSchedule-Active]
    where Id = @ScheduleId and TypeId = @typeId
)
begin
    set @ScheduleId = newid()

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
        ,DayOfWeek
        ,Hour
        ,Minute
        ,Second
        ,TimeZoneId
        ,NextRun
    )
    values (
         @ScheduleId
        ,@ScheduleId
        ,@now
        ,@SessionId
        ,@name
        ,@typeId
        ,@config
        ,@description
        ,@DeviceId
        ,1
        ,3
        ,1
        ,0
        ,@ScheduleHour
        ,@ScheduleMinute
        ,0
        ,@ScheduleTimeZoneId
        ,@now
    )
end
else
begin
    update [Framework].[JobSchedule]
    set Updated = @now
        ,UpdatedBy = @SessionId
        ,Name = @name
        ,Config = @config
        ,Description = @description
        ,DeviceId = @DeviceId
        ,RecurrenceAmount = 1
        ,RecurrenceInterval = 3
        ,RecurrencePattern = 1
        ,Day = null
        ,DayOfWeek = 0
        ,Hour = @ScheduleHour
        ,Minute = @ScheduleMinute
        ,Second = 0
        ,TimeZoneId = @ScheduleTimeZoneId
        ,NextRun = case
            when DeviceId <> @DeviceId
              or Hour <> @ScheduleHour
              or Minute <> @ScheduleMinute
              or isnull(TimeZoneId, N'') <> isnull(@ScheduleTimeZoneId, N'')
                then @now
            else NextRun
        end
    where Id = @ScheduleId
        and VersionOf = Id
        and IsDeleted = 0
        and TypeId = @typeId
        and (
             Name <> @name
          or Config <> @config
          or Description <> @description
          or isnull(DeviceId, '00000000-0000-0000-0000-000000000000') <> @DeviceId
          or RecurrenceAmount <> 1
          or RecurrenceInterval <> 3
          or RecurrencePattern <> 1
          or Day is not null
          or DayOfWeek <> 0
          or isnull(Hour, -1) <> @ScheduleHour
          or isnull(Minute, -1) <> @ScheduleMinute
          or isnull(Second, -1) <> 0
          or isnull(TimeZoneId, N'') <> isnull(@ScheduleTimeZoneId, N'')
        )
end