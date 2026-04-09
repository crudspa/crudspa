create view [Framework].[Permission-Active] as

select permission.Id as Id
    ,permission.Name as Name
from [Framework].[Permission] permission
where 1=1
    and permission.IsDeleted = 0