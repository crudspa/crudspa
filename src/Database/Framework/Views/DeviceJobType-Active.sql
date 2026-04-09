create view [Framework].[DeviceJobType-Active] as

select deviceJobType.Id as Id
    ,deviceJobType.DeviceId as DeviceId
    ,deviceJobType.JobTypeId as JobTypeId
from [Framework].[DeviceJobType] deviceJobType
where 1=1
    and deviceJobType.IsDeleted = 0
    and deviceJobType.VersionOf = deviceJobType.Id