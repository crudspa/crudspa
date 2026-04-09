create view [Education].[Grade-Active] as

select grade.Id as Id
    ,grade.Name as Name
    ,grade.Ordinal as Ordinal
from [Education].[Grade] grade
where 1=1
    and grade.IsDeleted = 0