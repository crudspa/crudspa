create proc [ContentDesign].[CommentDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @postId uniqueidentifier
declare @threadId uniqueidentifier

select
     @postId = comment.PostId
    ,@threadId = coalesce(comment.ThreadId, openingThread.Id)
from [Content].[Comment-Active] comment
    left join [Content].[Thread-Active] openingThread on openingThread.CommentId = comment.Id
where comment.Id = @Id

declare @now datetimeoffset = sysdatetimeoffset()
declare @reactionIds table ([Id] uniqueidentifier not null primary key)

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Content].[Post-Active] post
        inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
        inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
    where post.Id = @postId
        and portal.OwnerId = @organizationId
)
and not exists (
    select 1
    from [Content].[Thread-Active] thread
        inner join [Content].[Forum-Active] forum on thread.ForumId = forum.Id
        inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
    where thread.Id = @threadId
        and portal.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

insert @reactionIds (Id)
select distinct commentReaction.ReactionId
from [Content].[CommentReaction-Active] commentReaction
where commentReaction.CommentId = @Id

update commentReaction
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[CommentReaction] commentReaction
    inner join [Content].[CommentReaction-Active] activeCommentReaction on activeCommentReaction.Id = commentReaction.Id
where activeCommentReaction.CommentId = @Id

update reaction
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Reaction] reaction
    inner join [Content].[Reaction-Active] activeReaction on activeReaction.Id = reaction.Id
    inner join @reactionIds reactionId on reactionId.Id = activeReaction.Id

update commentMedia
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[CommentMedia] commentMedia
    inner join [Content].[CommentMedia-Active] activeCommentMedia on activeCommentMedia.Id = commentMedia.Id
where activeCommentMedia.CommentId = @Id

update commentTag
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[CommentTag] commentTag
    inner join [Content].[CommentTag-Active] activeCommentTag on activeCommentTag.Id = commentTag.Id
where activeCommentTag.CommentId = @Id

if exists (select 1 from [Content].[Comment-Active] child where child.ParentId = @Id)
    or exists (select 1 from [Content].[Thread-Active] thread where thread.CommentId = @Id)
begin
    update comment
    set
         Updated = @now
        ,UpdatedBy = @SessionId
        ,Edited = @now
        ,Removed = 1
        ,Body = N'Comment removed.'
    from [Content].[Comment] comment
        inner join [Content].[Comment-Active] activeComment on activeComment.Id = comment.Id
    where comment.Id = @Id
end
else
begin
    update comment
    set
         IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    from [Content].[Comment] comment
        inner join [Content].[Comment-Active] activeComment on activeComment.Id = comment.Id
    where comment.Id = @Id
end

commit transaction