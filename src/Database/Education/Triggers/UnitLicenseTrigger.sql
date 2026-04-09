create trigger [Education].[UnitLicenseTrigger] on [Education].[UnitLicense]
    for update
as

insert [Education].[UnitLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,UnitId
    ,LicenseId
    ,AllBooks
    ,AllLessons
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.UnitId
    ,deleted.LicenseId
    ,deleted.AllBooks
    ,deleted.AllLessons
from deleted