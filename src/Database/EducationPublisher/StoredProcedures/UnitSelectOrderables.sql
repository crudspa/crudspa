create proc [EducationPublisher].[UnitSelectOrderables] as

set nocount on
select
     unit.Id
    ,unit.Title as Name
    ,unit.Ordinal
from [Education].[Unit-Active] unit
order by unit.Ordinal