create trigger [Content].[ForumLicenseTrigger] on [Content].[ForumLicense]
    for update
as

insert [Content].[ForumLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ForumId
    ,LicenseId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ForumId
    ,deleted.LicenseId
from deleted