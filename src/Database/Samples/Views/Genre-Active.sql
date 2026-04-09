create view [Samples].[Genre-Active] as

select genre.Id as Id
    ,genre.Name as Name
    ,genre.Ordinal as Ordinal
from [Samples].[Genre] genre
where 1=1
    and genre.IsDeleted = 0