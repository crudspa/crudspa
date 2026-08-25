create proc [ContentDisplay].[ForumRunCommentSelectTree] (
     @SessionId uniqueidentifier
    ,@ThreadId uniqueidentifier
    ,@LicenseIds [Framework].[IdList] readonly
) as
begin
    set nocount on;

    declare @completeStatusId uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c';
    declare @forumsPermissionId uniqueidentifier = '7e4b7332-0f6f-429a-a95e-d8a21a08e4b5';
    declare @sessionUserId uniqueidentifier;
    declare @sessionContactId uniqueidentifier;
    declare @sessionPortalId uniqueidentifier;

    select @sessionUserId = session.UserId, @sessionPortalId = session.PortalId, @sessionContactId = userTable.ContactId
    from [Framework].[Session-Active] session
    left join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    where session.Id = @SessionId and session.Ended is null;

    -- Runtime participants can modify only their own content; Publisher moderation uses ContentDesign.
    declare @canModerate bit = 0;

    if not exists (
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
    ) return;

    select
         comment.Id
        ,comment.ParentId
        ,comment.Body
        ,comment.ById
        ,concat(byTable.FirstName, case when byTable.LastName is null then '' else ' ' + byTable.LastName end)
        ,comment.ByOrganizationName
        ,comment.Posted
        ,comment.Edited
        ,comment.ThreadId
        ,comment.Removed
        ,convert(bit, case when comment.Removed = 0 and (@canModerate = 1 or comment.ById = @sessionContactId) then 1 else 0 end)
        ,convert(bit, case when comment.Removed = 0 and (@canModerate = 1 or comment.ById = @sessionContactId) then 1 else 0 end)
        ,convert(bit, case when comment.Removed = 0 then 1 else 0 end)
    from [Content].[Comment-Active] comment
    inner join [Framework].[Contact-Active] byTable on byTable.Id = comment.ById
    where comment.ThreadId = @ThreadId
    order by comment.Posted;

    select
         media.Id
        ,media.CommentId
        ,media.[Type]
        ,audio.Id, audio.BlobId, audio.[Name], audio.[Format], audio.OptimizedStatus, audio.OptimizedBlobId, audio.OptimizedFormat
        ,image.Id, image.BlobId, image.[Name], image.[Format], image.Width, image.Height, image.Caption
        ,pdf.Id, pdf.BlobId, pdf.[Name], pdf.[Format], pdf.[Description]
        ,video.Id, video.BlobId, video.[Name], video.[Format], video.Width, video.Height, video.OptimizedStatus, video.OptimizedBlobId, video.OptimizedFormat
        ,media.Ordinal
    from [Content].[CommentMedia-Active] media
    inner join [Content].[Comment-Active] comment on comment.Id = media.CommentId
    left join [Framework].[AudioFile-Active] audio on audio.Id = media.AudioId
    left join [Framework].[ImageFile-Active] image on image.Id = media.ImageId
    left join [Framework].[PdfFile-Active] pdf on pdf.Id = media.PdfId
    left join [Framework].[VideoFile-Active] video on video.Id = media.VideoId
    where comment.ThreadId = @ThreadId
    order by media.Ordinal;

    select
         commentReaction.CommentId
        ,reaction.Emoji
        ,count(1)
        ,convert(bit, max(case when reaction.ById = @sessionContactId then 1 else 0 end))
    from [Content].[CommentReaction-Active] commentReaction
    inner join [Content].[Reaction-Active] reaction on reaction.Id = commentReaction.ReactionId
    inner join [Content].[Comment-Active] comment on comment.Id = commentReaction.CommentId
    where comment.ThreadId = @ThreadId and reaction.Emoji is not null
    group by commentReaction.CommentId, reaction.Emoji
    order by commentReaction.CommentId, reaction.Emoji;

    select distinct
         commentTag.CommentId
        ,tag.Id
        ,tag.Title
    from [Content].[CommentTag-Active] commentTag
    inner join [Content].[Comment-Active] comment on comment.Id = commentTag.CommentId
    inner join [Content].[Thread-Active] thread on thread.Id = comment.ThreadId
    inner join [Content].[Tag-Active] tag on tag.Id = commentTag.TagId
    where comment.ThreadId = @ThreadId
        and exists (
            select 1
            from [Content].[ForumBundle-Active] forumBundle
            inner join [Content].[TagBundle-Active] tagBundle
                on tagBundle.BundleId = forumBundle.BundleId and tagBundle.TagId = tag.Id
            where forumBundle.ForumId = thread.ForumId
                and forumBundle.CommentRule in (1, 2)
        )
    order by commentTag.CommentId, tag.Title;
end