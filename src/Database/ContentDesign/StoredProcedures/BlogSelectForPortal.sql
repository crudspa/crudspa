create proc [ContentDesign].[BlogSelectForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     blog.Id
    ,blog.PortalId
    ,blog.Title
    ,blog.StatusId
    ,status.Name as StatusName
    ,blog.Author
    ,blog.Description
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,(select count(1) from [Content].[Post-Active] where BlogId = blog.Id) as PostCount
from [Content].[Blog-Active] blog
    left join [Framework].[ImageFile-Active] image on blog.ImageId = image.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on blog.StatusId = status.Id
where blog.PortalId = @PortalId
    and organization.Id = @organizationId