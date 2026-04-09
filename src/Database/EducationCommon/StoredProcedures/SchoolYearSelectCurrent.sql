create proc [EducationCommon].[SchoolYearSelectCurrent]
as

declare @now datetimeoffset = sysdatetimeoffset()

select top 1
    schoolYear.Id as Id
    ,schoolYear.Name as Name
    ,schoolYear.Starts as Starts
    ,schoolYear.Ends as Ends
from [Education].[SchoolYear-Active] schoolYear
where schoolYear.Starts <= @now
    and schoolYear.Ends > @now
order by Starts desc