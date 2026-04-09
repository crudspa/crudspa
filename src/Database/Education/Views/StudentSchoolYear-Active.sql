create view [Education].[StudentSchoolYear-Active] as

select studentSchoolYear.Id as Id
    ,studentSchoolYear.StudentId as StudentId
    ,studentSchoolYear.SchoolYearId as SchoolYearId
from [Education].[StudentSchoolYear] studentSchoolYear
where 1=1
    and studentSchoolYear.IsDeleted = 0
    and studentSchoolYear.VersionOf = studentSchoolYear.Id