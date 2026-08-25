create proc [ContentDisplay].[ForumRunThreadDelete] (
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

    declare @lockedThreadId uniqueidentifier;
    select @lockedThreadId = Id
    from [Content].[Thread] with (updlock, holdlock)
    where Id = @Id and VersionOf = Id and IsDeleted = 0;

    -- Runtime participants can modify only their own content; Publisher moderation uses ContentDesign.
    declare @canModerate bit = 0;
    declare @openingCommentId uniqueidentifier;

    select @openingCommentId = thread.CommentId
    from [Content].[Thread-Active] thread
    inner join [Content].[Comment-Active] comment on comment.Id = thread.CommentId
    inner join [Content].[Forum-Active] forum on forum.Id = thread.ForumId
    where thread.Id = @Id
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
        and (
            @canModerate = 1
            or (
                comment.ById = @sessionContactId
                and not exists (
                    select 1 from [Content].[Comment-Active] reply where reply.ThreadId = thread.Id
                )
            )
        )
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

    if @openingCommentId is null throw 51000, 'Thread was not found or cannot be deleted.', 1;

    declare @now datetimeoffset(7) = sysdatetimeoffset();
    declare @commentIds table (Id uniqueidentifier primary key);
    insert @commentIds (Id) values (@openingCommentId);
    insert @commentIds (Id) select Id from [Content].[Comment-Active] where ThreadId = @Id;

    update commentReaction set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    from [Content].[CommentReaction] commentReaction
    inner join @commentIds commentId on commentId.Id = commentReaction.CommentId
    where commentReaction.VersionOf = commentReaction.Id and commentReaction.IsDeleted = 0;

    update reaction set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    from [Content].[Reaction] reaction
    inner join [Content].[CommentReaction] commentReaction on commentReaction.ReactionId = reaction.Id
    inner join @commentIds commentId on commentId.Id = commentReaction.CommentId
    where reaction.VersionOf = reaction.Id and reaction.IsDeleted = 0;

    update media set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    from [Content].[CommentMedia] media
    inner join @commentIds commentId on commentId.Id = media.CommentId
    where media.VersionOf = media.Id and media.IsDeleted = 0;

    update threadTag set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    from [Content].[ThreadTag] threadTag
    where threadTag.ThreadId = @Id and threadTag.VersionOf = threadTag.Id and threadTag.IsDeleted = 0;

    update commentTag set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    from [Content].[CommentTag] commentTag
    inner join @commentIds commentId on commentId.Id = commentTag.CommentId
    where commentTag.VersionOf = commentTag.Id and commentTag.IsDeleted = 0;

    update comment set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    from [Content].[Comment] comment
    inner join @commentIds commentId on commentId.Id = comment.Id
    where comment.VersionOf = comment.Id and comment.IsDeleted = 0;

    update [Content].[Thread]
    set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    where Id = @Id;
end