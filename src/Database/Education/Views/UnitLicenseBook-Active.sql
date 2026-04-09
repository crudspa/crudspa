create view [Education].[UnitLicenseBook-Active] as

select unitLicenseBook.Id as Id
    ,unitLicenseBook.UnitLicenseId as UnitLicenseId
    ,unitLicenseBook.BookId as BookId
from [Education].[UnitLicenseBook] unitLicenseBook
where 1=1
    and unitLicenseBook.IsDeleted = 0
    and unitLicenseBook.VersionOf = unitLicenseBook.Id