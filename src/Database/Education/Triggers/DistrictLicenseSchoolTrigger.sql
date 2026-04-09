create trigger [Education].[DistrictLicenseSchoolTrigger] on [Education].[DistrictLicenseSchool]
    for update
as

insert [Education].[DistrictLicenseSchool] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,DistrictLicenseId
    ,SchoolId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.DistrictLicenseId
    ,deleted.SchoolId
from deleted