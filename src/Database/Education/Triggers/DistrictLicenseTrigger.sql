create trigger [Education].[DistrictLicenseTrigger] on [Education].[DistrictLicense]
    for update
as

insert [Education].[DistrictLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,DistrictId
    ,LicenseId
    ,AllSchools
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.DistrictId
    ,deleted.LicenseId
    ,deleted.AllSchools
from deleted