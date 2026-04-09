create view [Framework].[JobStatus-Active] as

select jobStatus.Id as Id
    ,jobStatus.Name as Name
    ,jobStatus.CssClass as CssClass
    ,jobStatus.Ordinal as Ordinal
from [Framework].[JobStatus] jobStatus
where 1=1
    and jobStatus.IsDeleted = 0