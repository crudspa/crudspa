create proc [EducationCommon].[SchoolYearSelectPrevious]
as

declare @now datetimeoffset = sysdatetimeoffset()

select top 1
    schoolYear.Id as Id
    ,schoolYear.Name as Name
    ,schoolYear.Starts as Starts
    ,schoolYear.Ends as Ends
from [Education].[SchoolYear-Active] schoolYear
where schoolYear.Ends <= @now
order by Starts desc