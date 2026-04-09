create proc [FrameworkJobs].[JobScheduleSelectNames] as

select
     jobSchedule.Id
    ,jobSchedule.Name
from [Framework].[JobSchedule-Active] jobSchedule
order by jobSchedule.Name