create view [Education].[SchoolYear-Active] as

select schoolYear.Id as Id
    ,schoolYear.Name as Name
    ,schoolYear.Starts as Starts
    ,schoolYear.Ends as Ends
from [Education].[SchoolYear] schoolYear
where 1=1
    and schoolYear.IsDeleted = 0