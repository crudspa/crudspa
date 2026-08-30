create proc [EducationRostering].[RosterScheduleDelete] (
     @SessionId uniqueidentifier
    ,@ScheduleId uniqueidentifier
) as

set xact_abort on

declare @pending uniqueidentifier = '5e2d54a0-5774-4cae-8391-0b6ac31d4f60'

update job
set IsDeleted = 1
from [Framework].[Job] job
inner join [Framework].[JobSchedule-Active] schedule on schedule.Id = job.ScheduleId
inner join [Framework].[JobType-Active] type on type.Id = schedule.TypeId and type.Name = N'Roster Sync'
where job.ScheduleId = @ScheduleId
    and job.StatusId = @pending
    and job.IsDeleted = 0

update schedule
set Updated = sysdatetimeoffset()
    ,UpdatedBy = @SessionId
    ,IsDeleted = 1
from [Framework].[JobSchedule] schedule
inner join [Framework].[JobType-Active] type on type.Id = schedule.TypeId and type.Name = N'Roster Sync'
where schedule.Id = @ScheduleId
    and schedule.VersionOf = schedule.Id
    and schedule.IsDeleted = 0