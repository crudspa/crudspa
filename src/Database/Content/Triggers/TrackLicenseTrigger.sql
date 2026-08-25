create trigger [Content].[TrackLicenseTrigger] on [Content].[TrackLicense]
    for update
as

insert [Content].[TrackLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,TrackId
    ,LicenseId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.TrackId
    ,deleted.LicenseId
from deleted