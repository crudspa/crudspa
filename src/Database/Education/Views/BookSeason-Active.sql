create view [Education].[BookSeason-Active] as

select bookSeason.Id as Id
    ,bookSeason.Name as Name
    ,bookSeason.Ordinal as Ordinal
from [Education].[BookSeason] bookSeason
where 1=1
    and bookSeason.IsDeleted = 0