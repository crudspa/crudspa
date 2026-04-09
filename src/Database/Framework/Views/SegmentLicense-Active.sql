create view [Framework].[SegmentLicense-Active] as

select segmentLicense.Id as Id
    ,segmentLicense.SegmentId as SegmentId
    ,segmentLicense.LicenseId as LicenseId
from [Framework].[SegmentLicense] segmentLicense
where 1=1
    and segmentLicense.IsDeleted = 0
    and segmentLicense.VersionOf = segmentLicense.Id