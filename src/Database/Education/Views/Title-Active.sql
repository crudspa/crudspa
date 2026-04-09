create view [Education].[Title-Active] as

select title.Id as Id
    ,title.Name as Name
    ,title.Ordinal as Ordinal
from [Education].[Title] title
where 1=1
    and title.IsDeleted = 0