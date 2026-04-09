create proc [ContentDesign].[PostUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(150)
    ,@StatusId uniqueidentifier
    ,@Author nvarchar(150)
    ,@Published date
    ,@Revised date
    ,@CommentRule int
    ,@PageTypeId uniqueidentifier
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

declare @pageId uniqueidentifier = (select top 1 PageId from [Content].[Post] where Id = @Id)

update [Content].[Page]
set
     Id = @pageId
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TypeId = @PageTypeId
where Id = @pageId

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,StatusId = @StatusId
    ,Author = @Author
    ,Published = @Published
    ,Revised = @Revised
    ,CommentRule = @CommentRule
from [Content].[Post] baseTable
    inner join [Content].[Post-Active] post on post.Id = baseTable.Id
    inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction