create view [Framework].[Organization-Active] as

select organization.Id as Id
    ,organization.Name as Name
    ,organization.TimeZoneId as TimeZoneId
from [Framework].[Organization] organization
where 1=1
    and organization.IsDeleted = 0
    and organization.VersionOf = organization.Id