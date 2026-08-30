create trigger [Education].[RosterSourceTrigger] on [Education].[RosterSource]
    for update
as

insert [Education].[RosterSource] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OrganizationId
    ,Provider
    ,Tenant
    ,ClientId
    ,ClientSecret
    ,TokenUrl
    ,BaseUrl
    ,Mode
    ,ScheduleId
    ,[Checkpoint]
    ,LastSucceeded
    ,Recurring
    ,ScheduleHour
    ,ScheduleMinute
    ,ScheduleTimeZoneId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OrganizationId
    ,deleted.Provider
    ,deleted.Tenant
    ,deleted.ClientId
    ,deleted.ClientSecret
    ,deleted.TokenUrl
    ,deleted.BaseUrl
    ,deleted.Mode
    ,deleted.ScheduleId
    ,deleted.[Checkpoint]
    ,deleted.LastSucceeded
    ,deleted.Recurring
    ,deleted.ScheduleHour
    ,deleted.ScheduleMinute
    ,deleted.ScheduleTimeZoneId
from deleted