create proc [ContentDesign].[PostSelectNames] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
)
as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     post.Id
    ,blog.Title + ' | ' + post.Title as Name
from [Content].[Post-Active] post
    inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where blog.PortalId = @PortalId
    and organization.Id = @organizationId
order by blog.Title, post.Title