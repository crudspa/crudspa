create proc [ContentDesign].[PostSelect] (
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
     post.Id
    ,post.BlogId
    ,post.Title
    ,post.StatusId
    ,status.Name as StatusName
    ,post.Author
    ,post.Published
    ,post.Revised
    ,post.CommentRule
    ,page.Id as PageId
    ,page.TypeId as PageTypeId
    ,type.Name as PageTypeName
    ,(select count(1) from [Content].[Comment-Active] where PostId = post.Id) as CommentCount
    ,(select count(1) from [Content].[PostReaction-Active] where PostId = post.Id) as PostReactionCount
    ,(select count(1) from [Content].[PostTag-Active] where PostId = post.Id) as PostTagCount
    ,(select count(1) from [Content].[Section-Active] where PageId = post.PageId) as SectionCount
from [Content].[Post-Active] post
    inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
    inner join [Content].[Page-Active] page on post.PageId = page.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on post.StatusId = status.Id
    inner join [Content].[PageType-Active] type on page.TypeId = type.Id
where post.Id = @Id
    and organization.Id = @organizationId