create view [Framework].[Role-Active] as

select role.Id as Id
    ,role.Name as Name
    ,role.OrganizationId as OrganizationId
from [Framework].[Role] role
where 1=1
    and role.IsDeleted = 0
    and role.VersionOf = role.Id