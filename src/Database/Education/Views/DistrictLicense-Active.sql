create view [Education].[DistrictLicense-Active] as

select districtLicense.Id as Id
    ,districtLicense.DistrictId as DistrictId
    ,districtLicense.LicenseId as LicenseId
    ,districtLicense.AllSchools as AllSchools
from [Education].[DistrictLicense] districtLicense
where 1=1
    and districtLicense.IsDeleted = 0
    and districtLicense.VersionOf = districtLicense.Id