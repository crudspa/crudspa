create proc [FrameworkJobs].[JobTypeSelectFull] as

select
     jobType.Id
    ,jobType.Name
    ,jobType.EditorView
    ,jobType.ActionClass
from [Framework].[JobType-Active] jobType
order by jobType.Name