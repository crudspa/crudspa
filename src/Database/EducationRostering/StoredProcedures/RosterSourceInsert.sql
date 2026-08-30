create proc [EducationRostering].[RosterSourceInsert] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@Provider nvarchar(75)
    ,@Tenant nvarchar(255)
    ,@ClientId nvarchar(255)
    ,@ClientSecret nvarchar(500)
    ,@TokenUrl nvarchar(500)
    ,@BaseUrl nvarchar(500)
    ,@Mode nvarchar(25)
    ,@ScheduleId uniqueidentifier
    ,@Recurring bit
    ,@ScheduleHour int
    ,@ScheduleMinute int
    ,@ScheduleTimeZoneId nvarchar(32)
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[RosterSource] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,OrganizationId
    ,Provider
    ,Tenant
    ,ClientId
    ,ClientSecret
    ,TokenUrl
    ,BaseUrl
    ,Mode
    ,ScheduleId
    ,Recurring
    ,ScheduleHour
    ,ScheduleMinute
    ,ScheduleTimeZoneId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@OrganizationId
    ,@Provider
    ,@Tenant
    ,@ClientId
    ,@ClientSecret
    ,@TokenUrl
    ,@BaseUrl
    ,@Mode
    ,@ScheduleId
    ,@Recurring
    ,@ScheduleHour
    ,@ScheduleMinute
    ,@ScheduleTimeZoneId
)