create proc [EducationPublisher].[PostSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @contactId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
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
    ,post.Type
    ,post.GradeId
    ,post.SubjectId
    ,trim(byTable.FirstName) as ByFirstName
    ,trim(byTable.LastName) as ByLastName
    ,grade.Name as GradeName
    ,subject.Name as SubjectName
    ,(select top 1 Character from [Education].[PostReaction] where PostId = @Id and ById = @contactId order by Reacted desc) as ReactionCharacter
from [Education].[Post-Active] post
    inner join [Framework].[Contact-Active] byTable on post.ById = byTable.Id
    left join [Education].[Grade-Active] grade on post.GradeId = grade.Id
    left join [Education].[ClassroomType-Active] subject on post.SubjectId = subject.Id
    left join [Framework].[AudioFile-Active] audio on post.AudioId = audio.Id
    left join [Framework].[ImageFile-Active] image on post.ImageId = image.Id
    left join [Framework].[PdfFile-Active] pdf on post.PdfId = pdf.Id
    left join [Framework].[VideoFile-Active] video on post.VideoId = video.Id
where post.Id = @Id