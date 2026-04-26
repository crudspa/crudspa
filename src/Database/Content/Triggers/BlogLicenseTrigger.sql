create trigger [Content].[BlogLicenseTrigger] on [Content].[BlogLicense]
    for update
as

insert [Content].[BlogLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BlogId
    ,LicenseId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BlogId
    ,deleted.LicenseId
from deleted