create view [Education].[RosterSource-Active] as

select rosterSource.Id as Id
    ,rosterSource.OrganizationId as OrganizationId
    ,rosterSource.Provider as Provider
    ,rosterSource.Tenant as Tenant
    ,rosterSource.ClientId as ClientId
    ,rosterSource.ClientSecret as ClientSecret
    ,rosterSource.TokenUrl as TokenUrl
    ,rosterSource.BaseUrl as BaseUrl
    ,rosterSource.Mode as Mode
    ,rosterSource.ScheduleId as ScheduleId
    ,rosterSource.[Checkpoint] as [Checkpoint]
    ,rosterSource.LastSucceeded as LastSucceeded
    ,rosterSource.Recurring as Recurring
    ,rosterSource.ScheduleHour as ScheduleHour
    ,rosterSource.ScheduleMinute as ScheduleMinute
    ,rosterSource.ScheduleTimeZoneId as ScheduleTimeZoneId
from [Education].[RosterSource] rosterSource
where 1=1
    and rosterSource.IsDeleted = 0
    and rosterSource.VersionOf = rosterSource.Id