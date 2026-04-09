create proc [ContentDesign].[PostSelectPageId] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

select
    post.Id
    ,post.PageId
from [Content].[Post-Active] post
    inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
where post.Id = @Id
    and portal.OwnerId = @organizationId