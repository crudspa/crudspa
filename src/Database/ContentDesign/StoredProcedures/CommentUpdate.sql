create proc [ContentDesign].[CommentUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Body nvarchar(max)
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

set nocount on
set xact_abort on
begin transaction

if @Body is null or len(@Body) = 0
begin
    rollback transaction
    raiserror('Comment body is required.', 16, 1)
    return
end

if @threadId is not null and datalength(@Body) > 20000
begin
    rollback transaction
    raiserror('Forum comment body cannot exceed 10,000 characters.', 16, 1)
    return
end

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

update comment
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,Edited = @now
    ,Body = @Body
from [Content].[Comment] comment
    inner join [Content].[Comment-Active] activeComment on activeComment.Id = comment.Id
where comment.Id = @Id

commit transaction