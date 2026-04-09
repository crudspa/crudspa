create view [Framework].[Icon-Active] as

select icon.Id as Id
    ,icon.Name as Name
    ,icon.CssClass as CssClass
from [Framework].[Icon] icon
where 1=1
    and icon.IsDeleted = 0