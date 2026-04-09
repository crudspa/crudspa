create proc [EducationSchool].[SchoolYearSelectNames] as

select
    schoolYear.Id as Id
    ,schoolYear.[Name] as Name
from [Education].[SchoolYear-Active] as schoolYear
order by schoolYear.Starts desc