create proc [ContentDesign].[ForumDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @portalId uniqueidentifier = (select top 1 PortalId from [Content].[Forum-Active] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Content].[Forum-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()
declare @threadIds table (
     [Id] uniqueidentifier not null primary key
    ,[CommentId] uniqueidentifier not null
)
declare @commentIds table ([Id] uniqueidentifier not null primary key)
declare @reactionIds table ([Id] uniqueidentifier not null primary key)

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Content].[Forum-Active] forum
        inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
    where forum.Id = @Id
        and portal.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

insert @threadIds (Id, CommentId)
select thread.Id, thread.CommentId
from [Content].[Thread] thread
where thread.ForumId = @Id
    and thread.VersionOf = thread.Id

insert @commentIds (Id)
select thread.CommentId
from @threadIds thread
union
select comment.Id
from [Content].[Comment-Active] comment
    inner join @threadIds thread on thread.Id = comment.ThreadId

insert @reactionIds (Id)
select distinct commentReaction.ReactionId
from [Content].[CommentReaction-Active] commentReaction
    inner join @commentIds commentId on commentId.Id = commentReaction.CommentId

update forumLicense
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[ForumLicense] forumLicense
    inner join [Content].[ForumLicense-Active] activeForumLicense on activeForumLicense.Id = forumLicense.Id
where activeForumLicense.ForumId = @Id

update forumBundle
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[ForumBundle] forumBundle
    inner join [Content].[ForumBundle-Active] activeForumBundle on activeForumBundle.Id = forumBundle.Id
where activeForumBundle.ForumId = @Id

update threadTag
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[ThreadTag] threadTag
    inner join [Content].[ThreadTag-Active] activeThreadTag on activeThreadTag.Id = threadTag.Id
    inner join @threadIds thread on thread.Id = activeThreadTag.ThreadId

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
    inner join @threadIds threadId on threadId.Id = activeThread.Id

update forum
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Forum] forum
    inner join [Content].[Forum-Active] activeForum on activeForum.Id = forum.Id
where forum.Id = @Id

update forum
set forum.Ordinal = forum.Ordinal - 1
from [Content].[Forum] forum
    inner join [Content].[Forum-Active] activeForum on activeForum.Id = forum.Id
where activeForum.PortalId = @portalId
    and activeForum.Ordinal > @oldOrdinal

commit transaction