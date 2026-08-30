create proc [EducationRostering].[RosterSourceUpdate] (
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

update [Education].[RosterSource]
set Updated = sysdatetimeoffset()
    ,UpdatedBy = @SessionId
    ,Provider = @Provider
    ,Tenant = @Tenant
    ,ClientId = @ClientId
    ,ClientSecret = @ClientSecret
    ,TokenUrl = @TokenUrl
    ,BaseUrl = @BaseUrl
    ,Mode = @Mode
    ,ScheduleId = @ScheduleId
    ,Recurring = @Recurring
    ,ScheduleHour = @ScheduleHour
    ,ScheduleMinute = @ScheduleMinute
    ,ScheduleTimeZoneId = @ScheduleTimeZoneId
where Id = @Id
    and OrganizationId = @OrganizationId
    and VersionOf = Id
    and IsDeleted = 0

if @@rowcount = 0
    raiserror('Roster source not found', 16, 1)