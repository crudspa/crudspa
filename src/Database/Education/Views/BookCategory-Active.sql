create view [Education].[BookCategory-Active] as

select bookCategory.Id as Id
    ,bookCategory.Name as Name
    ,bookCategory.Ordinal as Ordinal
from [Education].[BookCategory] bookCategory
where 1=1
    and bookCategory.IsDeleted = 0