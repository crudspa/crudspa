create proc [ContentDisplay].[ForumRunCommentUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Body nvarchar(max)
    ,@TagIds [Framework].[IdList] readonly
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

    declare @forumId uniqueidentifier;

    select @forumId = forum.Id
        from [Content].[Comment-Active] comment
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
    ;

    if @forumId is null throw 51000, 'Comment was not found or cannot be edited.', 1;

    if @Body is null or len(@Body) = 0 or datalength(@Body) > 20000
        throw 51000, 'Comment body is required and cannot exceed 10,000 characters.', 1;

    if exists (
        select 1
        from @TagIds selectedTag
        where not exists (
            select 1
            from [Content].[ForumBundle-Active] forumBundle
            inner join [Content].[TagBundle-Active] tagBundle on tagBundle.BundleId = forumBundle.BundleId
            inner join [Content].[Tag-Active] tag on tag.Id = tagBundle.TagId
            where forumBundle.ForumId = @forumId
                and forumBundle.CommentRule in (1, 2)
                and tag.Id = selectedTag.Id
        )
    ) throw 51000, 'Selected comment tags are not permitted for this forum.', 1;

    if exists (
        select 1
        from [Content].[ForumBundle-Active] forumBundle
        where forumBundle.ForumId = @forumId
            and forumBundle.CommentRule = 2
            and not exists (
                select 1
                from @TagIds selectedTag
                inner join [Content].[TagBundle-Active] tagBundle
                    on tagBundle.TagId = selectedTag.Id and tagBundle.BundleId = forumBundle.BundleId
                inner join [Content].[Tag-Active] tag on tag.Id = selectedTag.Id
            )
    ) throw 51000, 'At least one tag is required from each required comment tag bundle.', 1;

    declare @now datetimeoffset(7) = sysdatetimeoffset();
    update [Content].[Comment]
    set Updated = @now, UpdatedBy = @SessionId, Edited = @now, Body = @Body
    where Id = @Id;

    update commentTag
    set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    from [Content].[CommentTag] commentTag
    inner join [Content].[CommentTag-Active] activeCommentTag on activeCommentTag.Id = commentTag.Id
    left join @TagIds selectedTag on selectedTag.Id = activeCommentTag.TagId
    where activeCommentTag.CommentId = @Id and selectedTag.Id is null;

    insert [Content].[CommentTag] (Id, VersionOf, Updated, UpdatedBy, CommentId, TagId)
    select junction.Id, junction.Id, @now, @SessionId, @Id, selectedTag.Id
    from @TagIds selectedTag
    left join [Content].[CommentTag-Active] commentTag
        on commentTag.CommentId = @Id and commentTag.TagId = selectedTag.Id
    cross apply (select newid() as Id) junction
    where commentTag.Id is null;
end