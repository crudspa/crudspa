create view [Education].[Post-Active] as

select post.Id as Id
    ,post.ForumId as ForumId
    ,post.ParentId as ParentId
    ,post.Pinned as Pinned
    ,post.Body as Body
    ,post.AudioId as AudioId
    ,post.ImageId as ImageId
    ,post.PdfId as PdfId
    ,post.VideoId as VideoId
    ,post.ById as ById
    ,post.ByOrganizationName as ByOrganizationName
    ,post.Posted as Posted
    ,post.Edited as Edited
    ,post.Type as Type
    ,post.GradeId as GradeId
    ,post.SubjectId as SubjectId
from [Education].[Post] post
where 1=1
    and post.IsDeleted = 0
    and post.VersionOf = post.Id