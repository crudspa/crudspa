create proc [EducationPublisher].[ForumLicenseSelect] (
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
     forumLicense.Id
    ,forumLicense.LicenseId
    ,forumLicense.ForumId
    ,forum.Title as ForumTitle
from [Content].[ForumLicense-Active] forumLicense
    inner join [Content].[Forum-Active] forum on forumLicense.ForumId = forum.Id
    inner join [Framework].[License-Active] license on forumLicense.LicenseId = license.Id
    inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
where forumLicense.Id = @Id
    and portal.OwnerId = @organizationId
    and license.OwnerId = @organizationId