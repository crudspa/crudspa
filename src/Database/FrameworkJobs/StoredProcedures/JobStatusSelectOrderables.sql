create proc [FrameworkJobs].[JobStatusSelectOrderables] as

select
     jobStatus.Id
    ,jobStatus.Name
    ,jobStatus.Ordinal
    ,jobStatus.CssClass
from [Framework].[JobStatus-Active] jobStatus
order by jobStatus.Ordinal