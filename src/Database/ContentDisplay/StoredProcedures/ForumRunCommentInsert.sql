create proc [ContentDisplay].[ForumRunCommentInsert] (
     @SessionId uniqueidentifier
    ,@ThreadId uniqueidentifier
    ,@ParentId uniqueidentifier
    ,@Body nvarchar(max)
    ,@TagIds [Framework].[IdList] readonly
    ,@LicenseIds [Framework].[IdList] readonly
    ,@Id uniqueidentifier output
) as
begin
    set nocount on;
    set xact_abort on;

    declare @completeStatusId uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c';
    declare @forumsPermissionId uniqueidentifier = '7e4b7332-0f6f-429a-a95e-d8a21a08e4b5';
    declare @sessionUserId uniqueidentifier;
    declare @sessionContactId uniqueidentifier;
    declare @sessionPortalId uniqueidentifier;
    declare @sessionOrganizationName nvarchar(75);

    select @sessionUserId = session.UserId, @sessionPortalId = session.PortalId,
        @sessionContactId = userTable.ContactId, @sessionOrganizationName = organization.Name
    from [Framework].[Session-Active] session
    inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    inner join [Framework].[Organization-Active] organization on organization.Id = userTable.OrganizationId
    where session.Id = @SessionId and session.Ended is null;

    declare @lockedThreadId uniqueidentifier;
    select @lockedThreadId = Id
    from [Content].[Thread] with (updlock, holdlock)
    where Id = @ThreadId and VersionOf = Id and IsDeleted = 0;

    -- Runtime participants can modify only their own content; Publisher moderation uses ContentDesign.
    declare @canModerate bit = 0;

    if @sessionContactId is null or not exists (
        select 1 from [Content].[Thread-Active] thread
        inner join [Content].[Forum-Active] forum on forum.Id = thread.ForumId
        where thread.Id = @ThreadId
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
            )))
    ) throw 51000, 'Thread was not found or is not accessible.', 1;

    if @Body is null or len(@Body) = 0 or datalength(@Body) > 20000
        throw 51000, 'Comment body is required and cannot exceed 10,000 characters.', 1;

    if (select count(1) from [Content].[Comment-Active] where ThreadId = @ThreadId) >= 200
        throw 51000, 'This discussion has reached its 200-reply limit. Start a continuation thread to keep the conversation going.', 1;

    if @ParentId is not null and not exists (
        select 1 from [Content].[Comment-Active] parent
        where parent.Id = @ParentId and parent.ThreadId = @ThreadId and parent.Removed = 0
    ) throw 51000, 'Reply parent is not in this thread.', 1;

    if @ParentId is not null
    begin
        declare @ancestorId uniqueidentifier = @ParentId;
        declare @parentDepth int = 0;

        while @ancestorId is not null
        begin
            set @parentDepth += 1;
            if @parentDepth >= 8
                throw 51000, 'Replies can be nested at most 8 levels deep.', 1;

            declare @nextAncestorId uniqueidentifier = null;
            select @nextAncestorId = ParentId
            from [Content].[Comment-Active]
            where Id = @ancestorId and ThreadId = @ThreadId;

            set @ancestorId = @nextAncestorId;
        end
    end

    declare @forumId uniqueidentifier = (
        select thread.ForumId
        from [Content].[Thread-Active] thread
        where thread.Id = @ThreadId
    );

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

    set @Id = newid();
    declare @now datetimeoffset(7) = sysdatetimeoffset();

    insert [Content].[Comment] (Id, VersionOf, Updated, UpdatedBy, ParentId, ThreadId, ById, ByOrganizationName, Body)
    values (@Id, @Id, @now, @SessionId, @ParentId, @ThreadId, @sessionContactId, @sessionOrganizationName, @Body);

    insert [Content].[CommentTag] (Id, VersionOf, Updated, UpdatedBy, CommentId, TagId)
    select junction.Id, junction.Id, @now, @SessionId, @Id, selectedTag.Id
    from @TagIds selectedTag
    cross apply (select newid() as Id) junction;
end