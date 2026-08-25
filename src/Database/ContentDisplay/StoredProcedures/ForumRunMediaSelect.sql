create proc [ContentDisplay].[ForumRunMediaSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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
         case media.[Type] when 0 then audio.BlobId when 1 then image.BlobId when 2 then pdf.BlobId when 3 then video.BlobId end
        ,case media.[Type] when 0 then audio.[Name] when 1 then image.[Name] when 2 then pdf.[Name] when 3 then video.[Name] end
        ,case media.[Type] when 0 then audio.[Format] when 1 then image.[Format] when 2 then pdf.[Format] when 3 then video.[Format] end
    from [Content].[CommentMedia-Active] media
    inner join [Content].[Comment-Active] comment on comment.Id = media.CommentId
    inner join [Content].[Thread-Active] thread on thread.Id = comment.ThreadId or thread.CommentId = comment.Id
    inner join [Content].[Forum-Active] forum on forum.Id = thread.ForumId
    left join [Framework].[AudioFile-Active] audio on audio.Id = media.AudioId
    left join [Framework].[ImageFile-Active] image on image.Id = media.ImageId
    left join [Framework].[PdfFile-Active] pdf on pdf.Id = media.PdfId
    left join [Framework].[VideoFile-Active] video on video.Id = media.VideoId
    where media.Id = @Id and comment.Removed = 0
        and (
            forum.PortalId = @sessionPortalId
            or (forum.AccessMode = 1 and exists (
                select 1 from [Content].[ForumLicense-Active] crossPortalLicense
                inner join @LicenseIds licenseId on licenseId.Id = crossPortalLicense.LicenseId
                inner join [Framework].[License-Active] activeLicense on activeLicense.Id = licenseId.Id
                where crossPortalLicense.ForumId = forum.Id
            ))
        )
        and forum.StatusId = @completeStatusId
            and (forum.PermissionId is null or exists (
                select 1 from [Framework].[UserRole-Active] userRole
                inner join [Framework].[RolePermission-Active] rolePermission on rolePermission.RoleId = userRole.RoleId
                inner join [Framework].[PortalPermission-Active] portalPermission
                    on portalPermission.PermissionId = rolePermission.PermissionId and portalPermission.PortalId = @sessionPortalId
                where userRole.UserId = @sessionUserId and rolePermission.PermissionId = forum.PermissionId
        ))
        and (forum.AccessMode = 0 or (forum.AccessMode = 1 and exists (
            select 1 from [Content].[ForumLicense-Active] forumLicense
            inner join @LicenseIds licenseId on licenseId.Id = forumLicense.LicenseId
            inner join [Framework].[License-Active] activeLicense on activeLicense.Id = licenseId.Id
            where forumLicense.ForumId = forum.Id
        )));
end