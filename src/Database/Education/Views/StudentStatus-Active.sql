create view [Education].[StudentStatus-Active] as

select studentStatus.Id as Id
    ,studentStatus.Name as Name
    ,studentStatus.Ordinal as Ordinal
from [Education].[StudentStatus] studentStatus
where 1=1
    and studentStatus.IsDeleted = 0