create proc [ContentDisplay].[ForumRunCommentMediaDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@LicenseIds [Framework].[IdList] readonly
) as
begin
    set nocount on;
    set xact_abort on;

    declare @completeStatusId uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c';
    declare @forumsPermissionId uniqueidentifier = '7e4b7332-0f6f-429a-a95e-d8a21a08e4b5';
    declare @sessionUserId uniqueidentifier;
    declare @sessionContactId uniqueidentifier;
    declare @sessionPortalId uniqueidentifier;
    select @sessionUserId = session.UserId, @sessionPortalId = session.PortalId, @sessionContactId = userTable.ContactId
    from [Framework].[Session-Active] session
    inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    where session.Id = @SessionId and session.Ended is null;

    -- Runtime participants can modify only their own content; Publisher moderation uses ContentDesign.
    declare @canModerate bit = 0;

    if not exists (
        select 1 from [Content].[CommentMedia-Active] media
        inner join [Content].[Comment-Active] comment on comment.Id = media.CommentId
        inner join [Content].[Thread-Active] thread on thread.Id = comment.ThreadId or thread.CommentId = comment.Id
        inner join [Content].[Forum-Active] forum on forum.Id = thread.ForumId
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
            and (@canModerate = 1 or comment.ById = @sessionContactId)
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
            )))
    ) throw 51000, 'Comment media cannot be deleted.', 1;

    declare @now datetimeoffset(7) = sysdatetimeoffset();
    update [Content].[CommentMedia]
    set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    where Id = @Id;
end