create view [Education].[DistrictLicenseSchool-Active] as

select districtLicenseSchool.Id as Id
    ,districtLicenseSchool.DistrictLicenseId as DistrictLicenseId
    ,districtLicenseSchool.SchoolId as SchoolId
from [Education].[DistrictLicenseSchool] districtLicenseSchool
where 1=1
    and districtLicenseSchool.IsDeleted = 0
    and districtLicenseSchool.VersionOf = districtLicenseSchool.Id