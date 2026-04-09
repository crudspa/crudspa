create proc [ContentDesign].[ThreadUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(150)
    ,@Pinned bit
    ,@CommentBody nvarchar(max)
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

declare @commentId uniqueidentifier = (select top 1 CommentId from [Content].[Thread] where Id = @Id)

update [Content].[Comment]
set
     Id = @commentId
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Body = @CommentBody
where Id = @commentId

update thread
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,Pinned = @Pinned
from [Content].[Thread] thread
where thread.Id = @Id

commit transaction