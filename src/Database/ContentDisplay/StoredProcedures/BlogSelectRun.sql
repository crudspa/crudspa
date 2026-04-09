create proc [ContentDisplay].[BlogSelectRun] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @portalId uniqueidentifier = (select top 1 PortalId from [Framework].[Session-Active] where Id = @SessionId)

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

insert [Content].[BlogViewed] (
    Id
    ,Updated
    ,UpdatedBy
    ,BlogId
)
values (
    newid()
    ,@now
    ,@SessionId
    ,@Id
)

select
    blog.Id
    ,blog.PortalId
    ,blog.StatusId
    ,blog.Title
    ,blog.Author
    ,blog.Description
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,status.Name as StatusName
    ,(select count(1) from [Content].[Post-Active] where BlogId = blog.Id) as PostCount
from [Content].[Blog-Active] blog
    inner join [Framework].[ContentStatus-Active] status on blog.StatusId = status.Id
    left join [Framework].[ImageFile-Active] image on blog.ImageId = image.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
where blog.Id = @Id
    and blog.StatusId = @ContentStatusComplete
    and portal.Id = @portalId

select
    post.Id
    ,post.BlogId
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
where post.BlogId = @Id
    and post.StatusId = @ContentStatusComplete
    and portal.Id = @portalId