create proc [ContentDesign].[ForumUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(150)
    ,@StatusId uniqueidentifier
    ,@PermissionId uniqueidentifier
    ,@AccessMode int
    ,@Description nvarchar(max)
    ,@ImageId uniqueidentifier
    ,@Licenses [Framework].[IdList] readonly
    ,@ForumBundles [Content].[ForumBundleList] readonly
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @portalId uniqueidentifier
declare @currentImageId uniqueidentifier

select
     @portalId = forum.PortalId
    ,@currentImageId = forum.ImageId
from [Content].[Forum] forum with (updlock, holdlock)
    inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
where forum.Id = @Id
    and forum.VersionOf = forum.Id
    and forum.IsDeleted = 0
    and portal.OwnerId = @organizationId

if @portalId is null
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if @ImageId is not null and (@currentImageId is null or @ImageId <> @currentImageId)
begin
    declare @coverBlobId uniqueidentifier

    select @coverBlobId = image.BlobId
    from [Framework].[ImageFile] image with (updlock, holdlock)
    where image.Id = @ImageId
        and image.IsDeleted = 0
        and image.UpdatedBy = @SessionId
        and image.Updated >= dateadd(minute, -30, @now)

    if @coverBlobId is null
    begin
        rollback transaction
        raiserror('Forum cover was not uploaded by this session or has expired', 16, 1)
        return
    end

    if exists (
        select 1
        from [Framework].[ImageFile] image with (updlock, holdlock)
        where image.Id <> @ImageId
            and image.BlobId = @coverBlobId
            and image.IsDeleted = 0
    ) or exists (
        select 1
        from [Content].[Forum] forum with (updlock, holdlock)
        where forum.VersionOf = forum.Id
            and forum.IsDeleted = 0
            and forum.ImageId = @ImageId
            and forum.Id <> @Id
    )
    begin
        rollback transaction
        raiserror('Forum cover file is already in use', 16, 1)
        return
    end
end

if @PermissionId is not null and not exists (
    select 1
    from [Framework].[Permission-Active] permission
        inner join [Framework].[PortalPermission-Active] portalPermission on portalPermission.PermissionId = permission.Id
    where permission.Id = @PermissionId
        and portalPermission.PortalId = @portalId
)
begin
    rollback transaction
    raiserror('Forum permission is not available for the portal', 16, 1)
    return
end

if @AccessMode not in (0, 1)
begin
    rollback transaction
    raiserror('Forum access mode is invalid', 16, 1)
    return
end

if @portalId = '2f6b54e3-689f-46a3-a1ee-30a4bfe18d63' and @AccessMode <> 0
begin
    rollback transaction
    raiserror('Provider forums support public access only', 16, 1)
    return
end

if @Description is null or len(@Description) = 0 or datalength(@Description) > 200000
begin
    rollback transaction
    raiserror('Forum description is required and cannot exceed 100,000 characters', 16, 1)
    return
end

if @AccessMode = 1 and not exists (select 1 from @Licenses)
begin
    rollback transaction
    raiserror('A licensed forum requires at least one license', 16, 1)
    return
end

if exists (
    select 1
    from @Licenses selectedLicense
        left join [Framework].[License-Active] license on license.Id = selectedLicense.Id
            and license.OwnerId = @organizationId
    where license.Id is null
)
begin
    rollback transaction
    raiserror('Forum license is not available for the portal owner', 16, 1)
    return
end

if exists (
    select 1
    from @ForumBundles forumBundle
    where forumBundle.ThreadRule not in (0, 1, 2)
        or forumBundle.CommentRule not in (0, 1, 2)
)
begin
    rollback transaction
    raiserror('Forum tag bundle rule is invalid', 16, 1)
    return
end

if exists (
    select 1
    from @ForumBundles forumBundle
        left join [Content].[Bundle-Active] bundle on bundle.Id = forumBundle.BundleId
    where bundle.Id is null
)
begin
    rollback transaction
    raiserror('Forum tag bundle is not available', 16, 1)
    return
end

if exists (
    select 1
    from @ForumBundles forumBundle
    where (forumBundle.ThreadRule = 2 or forumBundle.CommentRule = 2)
        and not exists (
            select 1
            from [Content].[TagBundle-Active] tagBundle
                inner join [Content].[Tag-Active] tag on tag.Id = tagBundle.TagId
            where tagBundle.BundleId = forumBundle.BundleId
        )
)
begin
    rollback transaction
    raiserror('A required forum tag bundle must contain at least one active tag', 16, 1)
    return
end

update forum
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,StatusId = @StatusId
    ,PermissionId = @PermissionId
    ,AccessMode = @AccessMode
    ,Description = @Description
    ,ImageId = @ImageId
from [Content].[Forum] forum
where forum.Id = @Id

update forumLicense
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[ForumLicense] forumLicense
    inner join [Content].[ForumLicense-Active] activeForumLicense on activeForumLicense.Id = forumLicense.Id
    left join @Licenses selectedLicense on selectedLicense.Id = activeForumLicense.LicenseId
where activeForumLicense.ForumId = @Id
    and selectedLicense.Id is null

insert [Content].[ForumLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ForumId
    ,LicenseId
)
select
     newRow.JunctionId
    ,newRow.JunctionId
    ,@now
    ,@SessionId
    ,@Id
    ,selectedLicense.Id
from @Licenses selectedLicense
    left join [Content].[ForumLicense-Active] existingForumLicense on existingForumLicense.ForumId = @Id
        and existingForumLicense.LicenseId = selectedLicense.Id
    cross apply (select newid() as JunctionId) newRow
where existingForumLicense.Id is null

update forumBundle
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[ForumBundle] forumBundle
    inner join [Content].[ForumBundle-Active] activeForumBundle on activeForumBundle.Id = forumBundle.Id
    left join @ForumBundles selectedForumBundle on selectedForumBundle.BundleId = activeForumBundle.BundleId
        and (selectedForumBundle.ThreadRule <> 0 or selectedForumBundle.CommentRule <> 0)
where activeForumBundle.ForumId = @Id
    and selectedForumBundle.BundleId is null

update forumBundle
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,ThreadRule = selectedForumBundle.ThreadRule
    ,CommentRule = selectedForumBundle.CommentRule
from [Content].[ForumBundle] forumBundle
    inner join [Content].[ForumBundle-Active] activeForumBundle on activeForumBundle.Id = forumBundle.Id
    inner join @ForumBundles selectedForumBundle on selectedForumBundle.BundleId = activeForumBundle.BundleId
where activeForumBundle.ForumId = @Id
    and (selectedForumBundle.ThreadRule <> 0 or selectedForumBundle.CommentRule <> 0)
    and (
        activeForumBundle.ThreadRule <> selectedForumBundle.ThreadRule
        or activeForumBundle.CommentRule <> selectedForumBundle.CommentRule
    )

insert [Content].[ForumBundle] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ForumId
    ,BundleId
    ,ThreadRule
    ,CommentRule
)
select
     newRow.JunctionId
    ,newRow.JunctionId
    ,@now
    ,@SessionId
    ,@Id
    ,selectedForumBundle.BundleId
    ,selectedForumBundle.ThreadRule
    ,selectedForumBundle.CommentRule
from @ForumBundles selectedForumBundle
    left join [Content].[ForumBundle-Active] existingForumBundle on existingForumBundle.ForumId = @Id
        and existingForumBundle.BundleId = selectedForumBundle.BundleId
    cross apply (select newid() as JunctionId) newRow
where existingForumBundle.Id is null
    and (selectedForumBundle.ThreadRule <> 0 or selectedForumBundle.CommentRule <> 0)

commit transaction