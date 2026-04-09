create proc [EducationSchool].[SchoolSelectNamesForDistrict] (
     @DistrictId uniqueidentifier
) as

select
    school.Id
    ,organization.Name as Name
from [Education].[School-Active] as school
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
where school.DistrictId = @DistrictId
order by organization.Name