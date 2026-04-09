create view [Content].[Blog-Active] as

select blog.Id as Id
    ,blog.PortalId as PortalId
    ,blog.StatusId as StatusId
    ,blog.Title as Title
    ,blog.Author as Author
    ,blog.Description as Description
    ,blog.ImageId as ImageId
from [Content].[Blog] blog
where 1=1
    and blog.IsDeleted = 0
    and blog.VersionOf = blog.Id