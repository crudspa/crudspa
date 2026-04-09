create proc [ContentDisplay].[PostSelectRun] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @portalId uniqueidentifier = (select top 1 PortalId from [Framework].[Session-Active] where Id = @SessionId)

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

insert [Content].[PostViewed] (
    Id
    ,Updated
    ,UpdatedBy
    ,PostId
)
values (
    newid()
    ,@now
    ,@SessionId
    ,@Id
)

select
    post.Id
    ,post.BlogId
    ,post.PageId
    ,post.StatusId
    ,post.Title
    ,post.Author
    ,post.Published
    ,post.Revised
    ,post.CommentRule
    ,status.Name as StatusName
    ,(select count(1) from [Content].[Section-Active] where PageId = post.PageId) as SectionCount
    ,(select count(1) from [Content].[Comment-Active] where PostId = post.Id) as CommentCount
    ,(select count(1) from [Content].[PostReaction-Active] where PostId = post.Id) as PostReactionCount
    ,(select count(1) from [Content].[PostTag-Active] where PostId = post.Id) as PostTagCount
from [Content].[Post-Active] post
    left join [Framework].[ContentStatus-Active] status on post.StatusId = status.Id
    inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
where post.Id = @Id
    and post.StatusId = @ContentStatusComplete
    and portal.Id = @portalId