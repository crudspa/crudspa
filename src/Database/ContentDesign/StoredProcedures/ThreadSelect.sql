create proc [ContentDesign].[ThreadSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     thread.Id
    ,thread.ForumId
    ,forum.Title as ForumTitle
    ,thread.Title
    ,thread.Pinned
    ,comment.Id as CommentId
    ,comment.Body as CommentBody
    ,comment.ById as CommentById
    ,byTable.FirstName as ContactFirstName
    ,comment.Posted as CommentPosted
    ,comment.Edited as CommentEdited
    ,(select count(1) from [Content].[Comment-Active] where ThreadId = thread.Id) as CommentCount
from [Content].[Thread-Active] thread
    inner join [Content].[Comment-Active] comment on thread.CommentId = comment.Id
    inner join [Framework].[Contact-Active] byTable on comment.ById = byTable.Id
    inner join [Content].[Forum-Active] forum on thread.ForumId = forum.Id
    inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where thread.Id = @Id
    and organization.Id = @organizationId