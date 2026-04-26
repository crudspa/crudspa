create view [Content].[ForumLicense-Active] as

select forumLicense.Id as Id
    ,forumLicense.ForumId as ForumId
    ,forumLicense.LicenseId as LicenseId
from [Content].[ForumLicense] forumLicense
where 1=1
    and forumLicense.IsDeleted = 0
    and forumLicense.VersionOf = forumLicense.Id