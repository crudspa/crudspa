create proc [FrameworkJobs].[DeviceSelectNames] as

select
     device.Id
    ,device.Name
from [Framework].[Device-Active] device
order by device.Name