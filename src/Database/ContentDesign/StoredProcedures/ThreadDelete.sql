create proc [ContentDesign].[ThreadDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()
declare @commentIds table ([Id] uniqueidentifier not null primary key)
declare @reactionIds table ([Id] uniqueidentifier not null primary key)

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Content].[Thread-Active] thread
        inner join [Content].[Forum-Active] forum on thread.ForumId = forum.Id
        inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
    where thread.Id = @Id
        and portal.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

insert @commentIds (Id)
select thread.CommentId
from [Content].[Thread-Active] thread
where thread.Id = @Id
union
select comment.Id
from [Content].[Comment-Active] comment
where comment.ThreadId = @Id

insert @reactionIds (Id)
select distinct commentReaction.ReactionId
from [Content].[CommentReaction-Active] commentReaction
    inner join @commentIds commentId on commentId.Id = commentReaction.CommentId

update threadTag
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[ThreadTag] threadTag
    inner join [Content].[ThreadTag-Active] activeThreadTag on activeThreadTag.Id = threadTag.Id
where activeThreadTag.ThreadId = @Id

update commentReaction
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[CommentReaction] commentReaction
    inner join [Content].[CommentReaction-Active] activeCommentReaction on activeCommentReaction.Id = commentReaction.Id
    inner join @commentIds commentId on commentId.Id = activeCommentReaction.CommentId

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
    inner join @commentIds commentId on commentId.Id = activeCommentMedia.CommentId

update commentTag
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[CommentTag] commentTag
    inner join [Content].[CommentTag-Active] activeCommentTag on activeCommentTag.Id = commentTag.Id
    inner join @commentIds commentId on commentId.Id = activeCommentTag.CommentId

update comment
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Comment] comment
    inner join [Content].[Comment-Active] activeComment on activeComment.Id = comment.Id
    inner join @commentIds commentId on commentId.Id = activeComment.Id

update thread
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Thread] thread
    inner join [Content].[Thread-Active] activeThread on activeThread.Id = thread.Id
where thread.Id = @Id

commit transaction