create view [Education].[ContentGroup-Active] as

select contentGroup.Id as Id
    ,contentGroup.Name as Name
    ,contentGroup.Ordinal as Ordinal
from [Education].[ContentGroup] contentGroup
where 1=1
    and contentGroup.IsDeleted = 0