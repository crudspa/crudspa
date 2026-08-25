create view [Content].[TrackLicense-Active] as

select trackLicense.Id as Id
    ,trackLicense.TrackId as TrackId
    ,trackLicense.LicenseId as LicenseId
from [Content].[TrackLicense] trackLicense
where 1=1
    and trackLicense.IsDeleted = 0
    and trackLicense.VersionOf = trackLicense.Id