create view [Education].[ClassRecording-Active] as

select classRecording.Id as Id
    ,classRecording.AudioFileId as AudioFileId
    ,classRecording.ImageId as ImageId
    ,classRecording.Uploaded as Uploaded
    ,classRecording.UploadedBy as UploadedBy
    ,classRecording.CategoryId as CategoryId
    ,classRecording.Unit as Unit
    ,classRecording.Lesson as Lesson
    ,classRecording.TeacherNotes as TeacherNotes
from [Education].[ClassRecording] classRecording
where 1=1
    and classRecording.IsDeleted = 0
    and classRecording.VersionOf = classRecording.Id