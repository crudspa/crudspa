create proc [EducationSchool].[StudentSchoolYearSelectSelectionsByStudent] (
     @StudentId uniqueidentifier
) as

select distinct
     @StudentId as RootId
    ,schoolYear.Id as Id
    ,schoolYear.[Name] as Name
    ,convert(bit, case when studentSchoolYear.Id is null then 0 else 1 end) as Selected
    ,schoolYear.Starts
from [Education].[SchoolYear-Active] schoolYear
    left join [Education].[StudentSchoolYear-Active] studentSchoolYear on studentSchoolYear.SchoolYearId = schoolYear.Id
        and studentSchoolYear.StudentId = @StudentId
order by schoolYear.Starts desc