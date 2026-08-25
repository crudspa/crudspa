create proc [ContentDisplay].[ForumRunThreadInsert] (
     @SessionId uniqueidentifier
    ,@ForumId uniqueidentifier
    ,@Title nvarchar(150)
    ,@CommentBody nvarchar(max)
    ,@TagIds [Framework].[IdList] readonly
    ,@LicenseIds [Framework].[IdList] readonly
    ,@Id uniqueidentifier output
    ,@CommentId uniqueidentifier output
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

    -- Runtime participants can modify only their own content; Publisher moderation uses ContentDesign.
    declare @canModerate bit = 0;

    if @sessionContactId is null or not exists (
        select 1 from [Content].[Forum-Active] forum
        where forum.Id = @ForumId
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
    ) throw 51000, 'Forum was not found or is not accessible.', 1;

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
            where forumBundle.ForumId = @ForumId
                and forumBundle.ThreadRule in (1, 2)
                and tag.Id = selectedTag.Id
        )
    ) throw 51000, 'Selected thread tags are not permitted for this forum.', 1;

    if exists (
        select 1
        from [Content].[ForumBundle-Active] forumBundle
        where forumBundle.ForumId = @ForumId
            and forumBundle.ThreadRule = 2
            and not exists (
                select 1
                from @TagIds selectedTag
                inner join [Content].[TagBundle-Active] tagBundle
                    on tagBundle.TagId = selectedTag.Id and tagBundle.BundleId = forumBundle.BundleId
                inner join [Content].[Tag-Active] tag on tag.Id = selectedTag.Id
            )
    ) throw 51000, 'At least one tag is required from each required thread tag bundle.', 1;

    set @Id = newid();
    set @CommentId = newid();
    declare @now datetimeoffset(7) = sysdatetimeoffset();

    insert [Content].[Comment] (Id, VersionOf, Updated, UpdatedBy, ById, ByOrganizationName, Body)
    values (@CommentId, @CommentId, @now, @SessionId, @sessionContactId, @sessionOrganizationName, @CommentBody);

    insert [Content].[Thread] (Id, VersionOf, Updated, UpdatedBy, ForumId, Title, CommentId, Pinned)
    values (@Id, @Id, @now, @SessionId, @ForumId, @Title, @CommentId, 0);

    insert [Content].[ThreadTag] (Id, VersionOf, Updated, UpdatedBy, ThreadId, TagId)
    select junction.Id, junction.Id, @now, @SessionId, @Id, selectedTag.Id
    from @TagIds selectedTag
    cross apply (select newid() as Id) junction;
end