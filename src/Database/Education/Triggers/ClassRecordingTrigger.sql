create trigger [Education].[ClassRecordingTrigger] on [Education].[ClassRecording]
    for update
as

insert [Education].[ClassRecording] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AudioFileId
    ,ImageId
    ,Uploaded
    ,UploadedBy
    ,CategoryId
    ,Unit
    ,Lesson
    ,TeacherNotes
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.AudioFileId
    ,deleted.ImageId
    ,deleted.Uploaded
    ,deleted.UploadedBy
    ,deleted.CategoryId
    ,deleted.Unit
    ,deleted.Lesson
    ,deleted.TeacherNotes
from deleted