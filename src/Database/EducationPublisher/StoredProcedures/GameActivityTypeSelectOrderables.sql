create proc [EducationPublisher].[GameActivityTypeSelectOrderables] as

set nocount on
select
     gameActivityType.Id
    ,gameActivityType.Name as Name
    ,gameActivityType.Ordinal
from [Education].[GameActivityType-Active] gameActivityType
order by gameActivityType.Ordinal