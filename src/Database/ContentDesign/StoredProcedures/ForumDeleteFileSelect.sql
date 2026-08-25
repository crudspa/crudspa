create proc [ContentDesign].[ForumDeleteFileSelect] (
     @SessionId uniqueidentifier
    ,@ForumId uniqueidentifier
) as
begin
    set nocount on;

    declare @organizationId uniqueidentifier = (
        select top 1 userTable.OrganizationId
        from [Framework].[User-Active] userTable
            inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
        where session.Id = @SessionId
            and session.Ended is null
    )
    declare @threadIds table (
         [Id] uniqueidentifier not null primary key
        ,[CommentId] uniqueidentifier not null
    )
    declare @commentIds table ([Id] uniqueidentifier not null primary key)

    if not exists (
        select 1
        from [Content].[Forum] forum
            inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
        where forum.Id = @ForumId
            and forum.VersionOf = forum.Id
            and forum.IsDeleted = 1
            and portal.OwnerId = @organizationId
    )
    begin
        raiserror('Deleted forum tenancy check failed', 16, 1)
        return
    end

    insert @threadIds (Id, CommentId)
    select thread.Id, thread.CommentId
    from [Content].[Thread] thread
    where thread.ForumId = @ForumId
        and thread.VersionOf = thread.Id

    insert @commentIds (Id)
    select thread.CommentId
    from @threadIds thread
    union
    select comment.Id
    from [Content].[Comment] comment
        inner join @threadIds thread on thread.Id = comment.ThreadId
    where comment.VersionOf = comment.Id

    select case
        when forum.ImageId is not null
            and not exists (
                select 1
                from [Content].[Forum-Active] activeForum
                where activeForum.ImageId = forum.ImageId
            )
            and not exists (
                select 1
                from [Content].[CommentMedia-Active] activeMedia
                where activeMedia.[Type] = 1
                    and activeMedia.ImageId = forum.ImageId
            )
        then forum.ImageId
        end as CoverImageId
    from [Content].[Forum] forum
    where forum.Id = @ForumId
        and forum.VersionOf = forum.Id

    select distinct
         media.[Type]
        ,case media.[Type]
            when 0 then media.AudioId
            when 1 then media.ImageId
            when 2 then media.PdfId
            when 3 then media.VideoId
        end as FileId
    from [Content].[CommentMedia] media
        inner join @commentIds comment on comment.Id = media.CommentId
    where media.VersionOf = media.Id
        and media.IsDeleted = 1
        and not exists (
            select 1
            from [Content].[CommentMedia-Active] activeMedia
            where activeMedia.[Type] = media.[Type]
                and (
                    (media.[Type] = 0 and activeMedia.AudioId = media.AudioId)
                    or (media.[Type] = 1 and activeMedia.ImageId = media.ImageId)
                    or (media.[Type] = 2 and activeMedia.PdfId = media.PdfId)
                    or (media.[Type] = 3 and activeMedia.VideoId = media.VideoId)
                )
        )
        and (
            media.[Type] <> 1
            or not exists (
                select 1
                from [Content].[Forum-Active] activeForum
                where activeForum.ImageId = media.ImageId
            )
        )
end