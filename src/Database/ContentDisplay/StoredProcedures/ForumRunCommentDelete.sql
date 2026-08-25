create proc [ContentDisplay].[ForumRunCommentDelete] (
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

    declare @threadId uniqueidentifier;
    select @threadId = comment.ThreadId
    from [Content].[Comment-Active] comment
    inner join [Content].[Thread] thread with (updlock, holdlock)
        on thread.Id = comment.ThreadId and thread.VersionOf = thread.Id and thread.IsDeleted = 0
    where comment.Id = @Id;

    -- Runtime participants can modify only their own content; Publisher moderation uses ContentDesign.
    declare @canModerate bit = 0;

    if not exists (
        select 1 from [Content].[Comment-Active] comment
        inner join [Content].[Thread-Active] thread on thread.Id = comment.ThreadId
        inner join [Content].[Forum-Active] forum on forum.Id = thread.ForumId
        where comment.Id = @Id and comment.Removed = 0
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
    ) throw 51000, 'Comment was not found or cannot be deleted.', 1;

    declare @now datetimeoffset(7) = sysdatetimeoffset();

    update commentReaction set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    from [Content].[CommentReaction] commentReaction
    where commentReaction.CommentId = @Id and commentReaction.VersionOf = commentReaction.Id and commentReaction.IsDeleted = 0;

    update reaction set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    from [Content].[Reaction] reaction
    inner join [Content].[CommentReaction] commentReaction on commentReaction.ReactionId = reaction.Id
    where commentReaction.CommentId = @Id and reaction.VersionOf = reaction.Id and reaction.IsDeleted = 0;

    update [Content].[CommentMedia]
    set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    where CommentId = @Id and VersionOf = Id and IsDeleted = 0;

    update [Content].[CommentTag]
    set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    where CommentId = @Id and VersionOf = Id and IsDeleted = 0;

    if exists (select 1 from [Content].[Comment-Active] child where child.ParentId = @Id)
        update [Content].[Comment]
        set Removed = 1, Updated = @now, UpdatedBy = @SessionId, Edited = @now, Body = N'Comment removed.'
        where Id = @Id;
    else
        update [Content].[Comment]
        set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
        where Id = @Id;
end