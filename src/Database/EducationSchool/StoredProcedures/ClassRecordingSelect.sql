create proc [EducationSchool].[ClassRecordingSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @schoolContactId uniqueidentifier = (
    select top 1 schoolContact.Id
    from [Framework].[Session-Active] session
        inner join [Framework].[User-Active] userTable on session.UserId = userTable.Id
        inner join [Framework].[Contact-Active] contact on userTable.ContactId = contact.Id
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.ContactId = contact.Id
    where session.Id = @SessionId
)

set nocount on

select
     classRecording.Id
    ,classRecording.Uploaded
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,classRecording.CategoryId
    ,category.Name as CategoryName
    ,classRecording.Unit
    ,classRecording.Lesson
    ,classRecording.TeacherNotes
from [Education].[ClassRecording-Active] classRecording
    left join [Framework].[AudioFile-Active] audioFile on classRecording.AudioFileId = audioFile.Id
    inner join [Education].[ContentCategory-Active] category on classRecording.CategoryId = category.Id
    left join [Framework].[ImageFile-Active] image on classRecording.ImageId = image.Id
    inner join [Education].[SchoolContact-Active] schoolContact on classRecording.UploadedBy = schoolContact.Id
where classRecording.Id = @Id
    and schoolContact.Id = @schoolContactId