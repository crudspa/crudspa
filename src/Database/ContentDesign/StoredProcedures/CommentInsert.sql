create proc [ContentDesign].[CommentInsert] (
     @SessionId uniqueidentifier
    ,@PostId uniqueidentifier
    ,@ThreadId uniqueidentifier
    ,@ParentId uniqueidentifier
    ,@Body nvarchar(max)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()
declare @byId uniqueidentifier
declare @organizationId uniqueidentifier
declare @organizationName nvarchar(75)

select top 1
     @byId = userTable.ContactId
    ,@organizationId = userTable.OrganizationId
    ,@organizationName = organization.Name
from [Framework].[User-Active] userTable
    inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    inner join [Framework].[Organization-Active] organization on organization.Id = userTable.OrganizationId
where session.Id = @SessionId

set nocount on
set xact_abort on
begin transaction

if @byId is null or @organizationId is null
begin
    rollback transaction
    raiserror('Comment author not found for session', 16, 1)
    return
end

if @ParentId is not null
begin
    set @PostId = null
    set @ThreadId = null

    select
         @PostId = parent.PostId
        ,@ThreadId = coalesce(parent.ThreadId, openingThread.Id)
    from [Content].[Comment-Active] parent
        left join [Content].[Thread-Active] openingThread on openingThread.CommentId = parent.Id
    where parent.Id = @ParentId
end

if (@PostId is null and @ThreadId is null)
    or (@PostId is not null and @ThreadId is not null)
begin
    rollback transaction
    raiserror('Comment must target exactly one post or thread', 16, 1)
    return
end

if @Body is null or len(@Body) = 0
begin
    rollback transaction
    raiserror('Comment body is required.', 16, 1)
    return
end

if @ThreadId is not null and datalength(@Body) > 20000
begin
    rollback transaction
    raiserror('Forum comment body cannot exceed 10,000 characters.', 16, 1)
    return
end

if @PostId is not null and not exists (
    select 1
    from [Content].[Post-Active] post
        inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
        inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
    where post.Id = @PostId
        and portal.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if @ThreadId is not null and not exists (
    select 1
    from [Content].[Thread-Active] thread
        inner join [Content].[Forum-Active] forum on thread.ForumId = forum.Id
        inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
    where thread.Id = @ThreadId
        and portal.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if @ThreadId is not null and exists (
    select 1
    from [Content].[Thread-Active] thread
    inner join [Content].[ForumBundle-Active] forumBundle on forumBundle.ForumId = thread.ForumId
    where thread.Id = @ThreadId and forumBundle.CommentRule = 2
)
begin
    rollback transaction
    raiserror('Create replies with required tags from the destination portal.', 16, 1)
    return
end

if @ThreadId is not null
    and (select count(1) from [Content].[Comment-Active] where ThreadId = @ThreadId) >= 200
begin
    rollback transaction
    raiserror('This discussion has reached its 200-reply limit. Start a continuation thread to keep the conversation going.', 16, 1)
    return
end

if @ThreadId is not null and @ParentId is not null
begin
    declare @ancestorId uniqueidentifier = @ParentId
    declare @parentDepth int = 0

    while @ancestorId is not null
    begin
        set @parentDepth += 1

        if @parentDepth >= 8
        begin
            rollback transaction
            raiserror('Replies can be nested at most 8 levels deep.', 16, 1)
            return
        end

        declare @nextAncestorId uniqueidentifier = null
        select @nextAncestorId = ParentId
        from [Content].[Comment-Active]
        where Id = @ancestorId and ThreadId = @ThreadId

        set @ancestorId = @nextAncestorId
    end
end

insert [Content].[Comment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ById
    ,ByOrganizationName
    ,PostId
    ,ThreadId
    ,ParentId
    ,Body
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@byId
    ,@organizationName
    ,@PostId
    ,@ThreadId
    ,@ParentId
    ,@Body
)

commit transaction