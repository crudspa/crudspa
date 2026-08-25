create proc [EducationPublisher].[BlogLicenseSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     blogLicense.Id
    ,blogLicense.LicenseId
    ,blogLicense.BlogId
    ,blog.Title as BlogTitle
from [Content].[BlogLicense-Active] blogLicense
    inner join [Content].[Blog-Active] blog on blogLicense.BlogId = blog.Id
    inner join [Framework].[License-Active] license on blogLicense.LicenseId = license.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
where blogLicense.Id = @Id
    and license.OwnerId = @organizationId
    and portal.OwnerId = @organizationId