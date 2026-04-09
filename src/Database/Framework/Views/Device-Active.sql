create view [Framework].[Device-Active] as

select device.Id as Id
    ,device.Name as Name
from [Framework].[Device] device
where 1=1
    and device.IsDeleted = 0