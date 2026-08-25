create proc [ContentDesign].[ForumCoverFileAuthorize] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@ForumId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@BlobId uniqueidentifier
) as
begin
    set nocount on;

    declare @forumsPermissionId uniqueidentifier = '7e4b7332-0f6f-429a-a95e-d8a21a08e4b5';

    select convert(bit, case when @ImageId is not null and @BlobId is not null and exists (
        select 1
        from [Framework].[Session-Active] session
        inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
        inner join [Framework].[Portal-Active] targetPortal
            on targetPortal.Id = @PortalId and targetPortal.OwnerId = userTable.OrganizationId
        inner join [Framework].[ImageFile] image
            on image.Id = @ImageId and image.BlobId = @BlobId and image.IsDeleted = 0
        where session.Id = @SessionId and session.Ended is null
            and image.UpdatedBy = @SessionId
            and image.Updated >= dateadd(minute, -30, sysdatetimeoffset())
            and exists (
                select 1
                from [Framework].[UserRole-Active] userRole
                inner join [Framework].[RolePermission-Active] rolePermission
                    on rolePermission.RoleId = userRole.RoleId
                inner join [Framework].[PortalPermission-Active] portalPermission
                    on portalPermission.PermissionId = rolePermission.PermissionId
                    and portalPermission.PortalId = session.PortalId
                where userRole.UserId = session.UserId
                    and rolePermission.PermissionId = @forumsPermissionId
            )
            and exists (
                select 1
                from [Framework].[PortalPaneType-Active] portalPaneType
                where portalPaneType.PortalId = targetPortal.Id
                    and portalPaneType.TypeId = '730eb0d1-2e7d-47dc-8b50-7434bfb25b64'
            )
            and not exists (
                select 1
                from [Framework].[ImageFile] otherImage
                where otherImage.Id <> image.Id and otherImage.BlobId = image.BlobId
                    and otherImage.IsDeleted = 0
            )
            and not exists (
                select 1
                from [Content].[Forum-Active] otherForum
                where otherForum.ImageId = image.Id
                    and (@ForumId is null or otherForum.Id <> @ForumId)
            )
    ) then 1 else 0 end);
end