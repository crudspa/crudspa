create view [Education].[BookSchoolYear-Active] as

select bookSchoolYear.Id as Id
    ,bookSchoolYear.BookId as BookId
    ,bookSchoolYear.SchoolYearId as SchoolYearId
from [Education].[BookSchoolYear] bookSchoolYear
where 1=1
    and bookSchoolYear.IsDeleted = 0
    and bookSchoolYear.VersionOf = bookSchoolYear.Id