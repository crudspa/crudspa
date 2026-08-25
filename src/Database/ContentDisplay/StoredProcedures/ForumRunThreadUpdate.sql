create proc [ContentDisplay].[ForumRunThreadUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(150)
    ,@Pinned bit
    ,@CommentBody nvarchar(max)
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
    declare @commentId uniqueidentifier;
    declare @forumId uniqueidentifier;

    select @commentId = thread.CommentId, @forumId = thread.ForumId
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
        )));

    if @commentId is null throw 51000, 'Thread was not found or cannot be edited.', 1;

    if @CommentBody is null or len(@CommentBody) = 0 or datalength(@CommentBody) > 20000
        throw 51000, 'Thread body is required and cannot exceed 10,000 characters.', 1;

    if exists (
        select 1
        from @TagIds selectedTag
        where not exists (
            select 1
            from [Content].[ForumBundle-Active] forumBundle
            inner join [Content].[TagBundle-Active] tagBundle on tagBundle.BundleId = forumBundle.BundleId
            inner join [Content].[Tag-Active] tag on tag.Id = tagBundle.TagId
            where forumBundle.ForumId = @forumId
                and forumBundle.ThreadRule in (1, 2)
                and tag.Id = selectedTag.Id
        )
    ) throw 51000, 'Selected thread tags are not permitted for this forum.', 1;

    if exists (
        select 1
        from [Content].[ForumBundle-Active] forumBundle
        where forumBundle.ForumId = @forumId
            and forumBundle.ThreadRule = 2
            and not exists (
                select 1
                from @TagIds selectedTag
                inner join [Content].[TagBundle-Active] tagBundle
                    on tagBundle.TagId = selectedTag.Id and tagBundle.BundleId = forumBundle.BundleId
                inner join [Content].[Tag-Active] tag on tag.Id = selectedTag.Id
            )
    ) throw 51000, 'At least one tag is required from each required thread tag bundle.', 1;

    declare @now datetimeoffset(7) = sysdatetimeoffset();

    update [Content].[Comment]
    set Updated = @now, UpdatedBy = @SessionId, Edited = @now, Body = @CommentBody
    where Id = @commentId and Removed = 0;

    update [Content].[Thread]
    set Updated = @now, UpdatedBy = @SessionId, Title = @Title,
        Pinned = case when @canModerate = 1 then @Pinned else Pinned end
    where Id = @Id;

    update threadTag
    set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
    from [Content].[ThreadTag] threadTag
    inner join [Content].[ThreadTag-Active] activeThreadTag on activeThreadTag.Id = threadTag.Id
    left join @TagIds selectedTag on selectedTag.Id = activeThreadTag.TagId
    where activeThreadTag.ThreadId = @Id and selectedTag.Id is null;

    insert [Content].[ThreadTag] (Id, VersionOf, Updated, UpdatedBy, ThreadId, TagId)
    select junction.Id, junction.Id, @now, @SessionId, @Id, selectedTag.Id
    from @TagIds selectedTag
    left join [Content].[ThreadTag-Active] threadTag
        on threadTag.ThreadId = @Id and threadTag.TagId = selectedTag.Id
    cross apply (select newid() as Id) junction
    where threadTag.Id is null;
end