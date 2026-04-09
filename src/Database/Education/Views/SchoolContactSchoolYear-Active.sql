create view [Education].[SchoolContactSchoolYear-Active] as

select schoolContactSchoolYear.Id as Id
    ,schoolContactSchoolYear.SchoolContactId as SchoolContactId
    ,schoolContactSchoolYear.SchoolYearId as SchoolYearId
from [Education].[SchoolContactSchoolYear] schoolContactSchoolYear
where 1=1
    and schoolContactSchoolYear.IsDeleted = 0
    and schoolContactSchoolYear.VersionOf = schoolContactSchoolYear.Id