create view [Content].[Bundle-Active] as

select bundle.Id as Id
    ,bundle.Name as Name
from [Content].[Bundle] bundle
where 1=1
    and bundle.IsDeleted = 0
    and bundle.VersionOf = bundle.Id