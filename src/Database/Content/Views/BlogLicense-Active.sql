create view [Content].[BlogLicense-Active] as

select blogLicense.Id as Id
    ,blogLicense.BlogId as BlogId
    ,blogLicense.LicenseId as LicenseId
from [Content].[BlogLicense] blogLicense
where 1=1
    and blogLicense.IsDeleted = 0
    and blogLicense.VersionOf = blogLicense.Id