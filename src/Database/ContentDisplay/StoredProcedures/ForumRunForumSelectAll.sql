create proc [ContentDisplay].[ForumRunForumSelectAll] (
     @SessionId uniqueidentifier
    ,@LicenseIds [Framework].[IdList] readonly
) as
begin
    set nocount on;

    declare @completeStatusId uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c';
    declare @forumsPermissionId uniqueidentifier = '7e4b7332-0f6f-429a-a95e-d8a21a08e4b5';
    declare @sessionUserId uniqueidentifier;
    declare @sessionPortalId uniqueidentifier;

    select @sessionUserId = session.UserId, @sessionPortalId = session.PortalId
    from [Framework].[Session-Active] session
    left join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    where session.Id = @SessionId and session.Ended is null;

    -- Runtime participants can modify only their own content; Publisher moderation uses ContentDesign.
    declare @canModerate bit = 0;

    select
         forum.Id
        ,forum.PortalId
        ,portal.[Key]
        ,forum.StatusId
        ,status.[Name]
        ,forum.Title
        ,forum.Description
        ,image.Id
        ,image.BlobId
        ,image.[Name]
        ,image.[Format]
        ,image.Width
        ,image.Height
        ,image.Caption
        ,forum.Ordinal
    from [Content].[Forum-Active] forum
    inner join [Framework].[Portal-Active] portal on portal.Id = forum.PortalId
    inner join [Framework].[ContentStatus-Active] status on status.Id = forum.StatusId
    left join [Framework].[ImageFile-Active] image on image.Id = forum.ImageId
    where (
            forum.PortalId = @sessionPortalId
            or (forum.AccessMode = 1 and exists (
                select 1 from [Content].[ForumLicense-Active] crossPortalLicense
                inner join @LicenseIds licenseId on licenseId.Id = crossPortalLicense.LicenseId
                inner join [Framework].[License-Active] activeLicense on activeLicense.Id = licenseId.Id
                where crossPortalLicense.ForumId = forum.Id
            ))
        )
        and forum.StatusId = @completeStatusId
        and (
            forum.PermissionId is null
            or exists (
                select 1 from [Framework].[UserRole-Active] userRole
                inner join [Framework].[RolePermission-Active] rolePermission on rolePermission.RoleId = userRole.RoleId
                inner join [Framework].[PortalPermission-Active] portalPermission
                    on portalPermission.PermissionId = rolePermission.PermissionId and portalPermission.PortalId = @sessionPortalId
                where userRole.UserId = @sessionUserId and rolePermission.PermissionId = forum.PermissionId
            )
        )
        and (
            forum.AccessMode = 0
            or (forum.AccessMode = 1 and exists (
                select 1 from [Content].[ForumLicense-Active] forumLicense
                inner join @LicenseIds licenseId on licenseId.Id = forumLicense.LicenseId
                inner join [Framework].[License-Active] activeLicense on activeLicense.Id = licenseId.Id
                where forumLicense.ForumId = forum.Id
            ))
        )
    order by forum.Ordinal, forum.Title;
end