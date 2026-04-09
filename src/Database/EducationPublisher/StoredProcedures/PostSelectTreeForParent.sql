create proc [EducationPublisher].[PostSelectTreeForParent] (
     @SessionId uniqueidentifier
    ,@ParentId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[PostViewed] (
    Id
    ,PostId
    ,Updated
    ,UpdatedBy
)
values (
    newid()
    ,@ParentId
    ,@now
    ,@SessionId
)

;with PostHierarchy as (
    select
        post.Id
        ,post.ParentId
    from [Education].[Post-Active] post
    where post.ParentId = @ParentId
    union all
    select
        childPost.Id
        ,childPost.ParentId
    from [Education].[Post-Active] childPost
        inner join PostHierarchy parentPost on childPost.ParentId = parentPost.Id
)
select
    post.Id
    ,post.ForumId
    ,post.ParentId
    ,post.Pinned
    ,post.Body
    ,audio.Id as AudioId
    ,audio.BlobId as AudioBlobId
    ,audio.Name as AudioName
    ,audio.Format as AudioFormat
    ,audio.OptimizedStatus as AudioOptimizedStatus
    ,audio.OptimizedBlobId as AudioOptimizedBlobId
    ,audio.OptimizedFormat as AudioOptimizedFormat
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,pdf.Id as PdfId
    ,pdf.BlobId as PdfBlobId
    ,pdf.Name as PdfName
    ,pdf.Format as PdfFormat
    ,pdf.Description as PdfDescription
    ,video.Id as VideoId
    ,video.BlobId as VideoBlobId
    ,video.Name as VideoName
    ,video.Format as VideoFormat
    ,video.OptimizedStatus as VideoOptimizedStatus
    ,video.OptimizedBlobId as VideoOptimizedBlobId
    ,video.OptimizedFormat as VideoOptimizedFormat
    ,post.ById
    ,post.ByOrganizationName
    ,post.Posted
    ,post.Edited
    ,trim(byTable.FirstName) as ByFirstName
    ,trim(byTable.LastName) as ByLastName
from PostHierarchy
    inner join [Education].[Post-Active] post on PostHierarchy.Id = post.Id
    inner join [Framework].[Contact-Active] byTable on post.ById = byTable.Id
    left join [Framework].[AudioFile-Active] audio on post.AudioId = audio.Id
    left join [Framework].[ImageFile-Active] image on post.ImageId = image.Id
    left join [Framework].[PdfFile-Active] pdf on post.PdfId = pdf.Id
    left join [Framework].[VideoFile-Active] video on post.VideoId = video.Id