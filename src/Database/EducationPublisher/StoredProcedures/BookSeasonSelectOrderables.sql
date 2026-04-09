create proc [EducationPublisher].[BookSeasonSelectOrderables] as

set nocount on
select
     bookSeason.Id
    ,bookSeason.Name as Name
    ,bookSeason.Ordinal
from [Education].[BookSeason-Active] bookSeason
order by bookSeason.Ordinal