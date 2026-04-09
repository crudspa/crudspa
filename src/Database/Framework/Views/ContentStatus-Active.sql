create view [Framework].[ContentStatus-Active] as

select contentStatus.Id as Id
    ,contentStatus.Name as Name
    ,contentStatus.Ordinal as Ordinal
from [Framework].[ContentStatus] contentStatus
where 1=1
    and contentStatus.IsDeleted = 0