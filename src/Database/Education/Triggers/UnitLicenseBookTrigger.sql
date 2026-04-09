create trigger [Education].[UnitLicenseBookTrigger] on [Education].[UnitLicenseBook]
    for update
as

insert [Education].[UnitLicenseBook] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,UnitLicenseId
    ,BookId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.UnitLicenseId
    ,deleted.BookId
from deleted