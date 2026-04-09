create view [Framework].[JobType-Active] as

select jobType.Id as Id
    ,jobType.Name as Name
    ,jobType.EditorView as EditorView
    ,jobType.ActionClass as ActionClass
from [Framework].[JobType] jobType
where 1=1
    and jobType.IsDeleted = 0