create view [Education].[BookStatus-Active] as

select bookStatus.Id as Id
    ,bookStatus.Name as Name
    ,bookStatus.Ordinal as Ordinal
from [Education].[BookStatus] bookStatus
where 1=1
    and bookStatus.IsDeleted = 0