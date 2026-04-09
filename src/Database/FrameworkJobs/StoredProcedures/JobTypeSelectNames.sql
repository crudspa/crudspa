create proc [FrameworkJobs].[JobTypeSelectNames] as

select
     jobType.Id
    ,jobType.Name
from [Framework].[JobType-Active] jobType
order by jobType.Name