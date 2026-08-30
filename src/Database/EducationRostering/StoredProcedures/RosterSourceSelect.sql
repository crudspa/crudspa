create proc [EducationRostering].[RosterSourceSelect] (
     @Id uniqueidentifier
) as

select
     rosterSource.Id
    ,rosterSource.OrganizationId
    ,rosterSource.Provider
    ,rosterSource.Tenant
    ,rosterSource.ClientId
    ,rosterSource.ClientSecret
    ,rosterSource.TokenUrl
    ,rosterSource.BaseUrl
    ,rosterSource.Mode
    ,rosterSource.ScheduleId
    ,rosterSource.[Checkpoint]
    ,rosterSource.LastSucceeded
    ,rosterSource.Recurring
    ,rosterSource.ScheduleHour
    ,rosterSource.ScheduleMinute
    ,rosterSource.ScheduleTimeZoneId
from [Education].[RosterSource-Active] rosterSource
where rosterSource.Id = @Id