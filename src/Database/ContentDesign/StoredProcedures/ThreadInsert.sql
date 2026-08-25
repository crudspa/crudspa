create proc [ContentDesign].[ThreadInsert] (
     @SessionId uniqueidentifier
    ,@ForumId uniqueidentifier
    ,@Title nvarchar(150)
    ,@Pinned bit
    ,@CommentBody nvarchar(max)
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

if (@byId is null)
begin
    rollback transaction
    raiserror('Comment author not found for session', 16, 1)
    return
end

if @CommentBody is null or len(@CommentBody) = 0 or datalength(@CommentBody) > 20000
begin
    rollback transaction
    raiserror('Thread body is required and cannot exceed 10,000 characters.', 16, 1)
    return
end

declare @commentId uniqueidentifier = newid()

insert [Content].[Comment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ById
    ,ByOrganizationName
    ,Body
)
values (
     @commentId
    ,@commentId
    ,@now
    ,@SessionId
    ,@byId
    ,@organizationName
    ,@CommentBody
)

insert [Content].[Thread] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ForumId
    ,Title
    ,Pinned
    ,CommentId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ForumId
    ,@Title
    ,@Pinned
    ,@commentId
)

if not exists (
    select 1
    from [Content].[Thread-Active] thread
        inner join [Content].[Forum-Active] forum on thread.ForumId = forum.Id
        inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where thread.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if exists (
    select 1
    from [Content].[ForumBundle-Active] forumBundle
    where forumBundle.ForumId = @ForumId and forumBundle.ThreadRule = 2
)
begin
    rollback transaction
    raiserror('Create threads with required tags from the destination portal.', 16, 1)
    return
end

commit transaction