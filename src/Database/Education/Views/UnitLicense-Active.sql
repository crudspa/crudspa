create view [Education].[UnitLicense-Active] as

select unitLicense.Id as Id
    ,unitLicense.UnitId as UnitId
    ,unitLicense.LicenseId as LicenseId
    ,unitLicense.AllBooks as AllBooks
    ,unitLicense.AllLessons as AllLessons
from [Education].[UnitLicense] unitLicense
where 1=1
    and unitLicense.IsDeleted = 0
    and unitLicense.VersionOf = unitLicense.Id