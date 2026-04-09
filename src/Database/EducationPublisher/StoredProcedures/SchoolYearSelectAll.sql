create proc [EducationPublisher].[SchoolYearSelectAll] as

select
    schoolYear.Id
    ,schoolYear.Name
    ,schoolYear.Starts
    ,schoolYear.Ends
from [Education].[SchoolYear-Active] schoolYear
order by schoolYear.Starts desc, schoolYear.Ends desc