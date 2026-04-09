create trigger [Framework].[SegmentLicenseTrigger] on [Framework].[SegmentLicense]
    for update
as

insert [Framework].[SegmentLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,SegmentId
    ,LicenseId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.SegmentId
    ,deleted.LicenseId
from deleted