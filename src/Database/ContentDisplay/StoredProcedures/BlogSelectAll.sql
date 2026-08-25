create proc [ContentDisplay].[BlogSelectAll] (
     @SessionId uniqueidentifier
    ,@LicenseIds [Framework].[IdList] readonly
) as

declare @portalId uniqueidentifier = (select top 1 PortalId from [Framework].[Session-Active] where Id = @SessionId)
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

select
    blog.Id
    ,blog.PortalId
    ,blog.StatusId
    ,blog.Title
    ,blog.Author
    ,blog.Description
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,status.Name as StatusName
    ,(select count(1) from [Content].[Post-Active] where BlogId = blog.Id) as PostCount
from [Content].[Blog-Active] blog
    inner join [Framework].[ContentStatus-Active] status on blog.StatusId = status.Id
    left join [Framework].[ImageFile-Active] image on blog.ImageId = image.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
where blog.StatusId = @ContentStatusComplete
    and (
        (portal.Id = @portalId and blog.AccessMode = 0)
        or (blog.AccessMode = 1 and exists (
            select 1
            from [Content].[BlogLicense-Active] blogLicense
                inner join @LicenseIds licenseId on licenseId.Id = blogLicense.LicenseId
                inner join [Framework].[License-Active] license on license.Id = licenseId.Id
            where blogLicense.BlogId = blog.Id
        ))
    )