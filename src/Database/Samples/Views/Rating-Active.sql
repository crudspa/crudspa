create view [Samples].[Rating-Active] as

select rating.Id as Id
    ,rating.Name as Name
    ,rating.Ordinal as Ordinal
from [Samples].[Rating] rating
where 1=1
    and rating.IsDeleted = 0