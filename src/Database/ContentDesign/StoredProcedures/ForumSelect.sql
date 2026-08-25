create proc [ContentDesign].[ForumSelect] (
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
     forum.Id
    ,forum.PortalId
    ,portal.[Key] as PortalKey
    ,forum.Title
    ,forum.StatusId
    ,status.Name as StatusName
    ,forum.Description
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,forum.PermissionId
    ,permission.Name as PermissionName
    ,forum.AccessMode
    ,forum.Ordinal
from [Content].[Forum-Active] forum
    left join [Framework].[ImageFile-Active] image on forum.ImageId = image.Id
    left join [Framework].[Permission-Active] permission on forum.PermissionId = permission.Id
    inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on forum.StatusId = status.Id
where forum.Id = @Id
    and organization.Id = @organizationId

select distinct
     @Id as ForumId
    ,license.Id as LicenseId
    ,license.Name as LicenseName
    ,convert(bit, iif(forumLicense.Id is null, 0, 1)) as Selected
from [Framework].[License-Active] license
    left join [Content].[ForumLicense-Active] forumLicense on forumLicense.LicenseId = license.Id
        and forumLicense.ForumId = @Id
where license.OwnerId = @organizationId
order by license.Name

select
     @Id as ForumId
    ,bundle.Id as BundleId
    ,bundle.Name as BundleName
    ,coalesce(forumBundle.ThreadRule, 0) as ThreadRule
    ,coalesce(forumBundle.CommentRule, 0) as CommentRule
from [Content].[Bundle-Active] bundle
    left join [Content].[ForumBundle-Active] forumBundle on forumBundle.BundleId = bundle.Id
        and forumBundle.ForumId = @Id
order by bundle.Name