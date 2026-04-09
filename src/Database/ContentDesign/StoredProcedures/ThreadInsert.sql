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

set nocount on
set xact_abort on
begin transaction

declare @commentId uniqueidentifier = newid()

insert [Content].[Comment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Body
)
values (
     @commentId
    ,@commentId
    ,@now
    ,@SessionId
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

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
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

commit transaction