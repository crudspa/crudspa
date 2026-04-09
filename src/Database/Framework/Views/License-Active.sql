create view [Framework].[License-Active] as

select license.Id as Id
    ,license.OwnerId as OwnerId
    ,license.Name as Name
    ,license.Description as Description
from [Framework].[License] license
where 1=1
    and license.IsDeleted = 0
    and license.VersionOf = license.Id