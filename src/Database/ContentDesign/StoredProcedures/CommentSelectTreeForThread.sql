create proc [ContentDesign].[CommentSelectTreeForThread] (
     @SessionId uniqueidentifier
    ,@ThreadId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

if not exists (
    select 1
    from [Content].[Thread-Active] thread
        inner join [Content].[Forum-Active] forum on thread.ForumId = forum.Id
        inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
    where thread.Id = @ThreadId
        and portal.OwnerId = @organizationId
)
return

set nocount on

select
     comment.Id
    ,comment.PostId
    ,comment.ParentId
    ,parent.Body as ParentBody
    ,comment.Body
    ,comment.ById
    ,concat(trim(byTable.FirstName), case when trim(byTable.LastName) = N'' then N'' else N' ' + trim(byTable.LastName) end) as ByName
    ,comment.ByOrganizationName
    ,comment.Posted
    ,comment.Edited
    ,comment.ThreadId
from [Content].[Comment-Active] comment
    inner join [Framework].[Contact-Active] byTable on comment.ById = byTable.Id
    left join [Content].[Comment-Active] parent on comment.ParentId = parent.Id
where comment.ThreadId = @ThreadId
order by comment.Posted

select
     commentMedia.Id
    ,commentMedia.CommentId
    ,comment.Body as CommentBody
    ,commentMedia.Type
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
    ,video.Width as VideoWidth
    ,video.Height as VideoHeight
    ,video.OptimizedStatus as VideoOptimizedStatus
    ,video.OptimizedBlobId as VideoOptimizedBlobId
    ,video.OptimizedFormat as VideoOptimizedFormat
    ,commentMedia.Ordinal
from [Content].[CommentMedia-Active] commentMedia
    left join [Framework].[AudioFile-Active] audio on commentMedia.AudioId = audio.Id
    inner join [Content].[Comment-Active] comment on commentMedia.CommentId = comment.Id
    left join [Framework].[ImageFile-Active] image on commentMedia.ImageId = image.Id
    left join [Framework].[PdfFile-Active] pdf on commentMedia.PdfId = pdf.Id
    left join [Framework].[VideoFile-Active] video on commentMedia.VideoId = video.Id
where comment.ThreadId = @ThreadId
order by commentMedia.Ordinal