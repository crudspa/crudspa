create proc [EducationDistrict].[SchoolContactSchoolYearSelectSelectionsBySchoolContact] (
     @SchoolContactId uniqueidentifier
) as

select distinct
     @SchoolContactId as RootId
    ,schoolYear.Id as Id
    ,schoolYear.[Name] as Name
    ,convert(bit, case when schoolContactSchoolYear.Id is null then 0 else 1 end) as Selected
    ,schoolYear.Starts
from [Education].[SchoolYear-Active] schoolYear
    left join [Education].[SchoolContactSchoolYear-Active] schoolContactSchoolYear on schoolContactSchoolYear.SchoolYearId = schoolYear.Id
        and schoolContactSchoolYear.SchoolContactId = @SchoolContactId
order by schoolYear.Starts desc