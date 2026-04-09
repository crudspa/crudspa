create proc [EducationSchool].[ClassRecordingInsert] (
     @SessionId uniqueidentifier
    ,@AudioFileId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@CategoryId uniqueidentifier
    ,@Unit int
    ,@Lesson int
    ,@TeacherNotes nvarchar(max)
    ,@Id uniqueidentifier output
) as

declare @schoolContactId uniqueidentifier = (
    select top 1 schoolContact.Id
    from [Framework].[Session-Active] session
        inner join [Framework].[User-Active] userTable on session.UserId = userTable.Id
        inner join [Framework].[Contact-Active] contact on userTable.ContactId = contact.Id
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.ContactId = contact.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[ClassRecording] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,AudioFileId
    ,ImageId
    ,CategoryId
    ,Unit
    ,Lesson
    ,TeacherNotes
    ,UploadedBy
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@AudioFileId
    ,@ImageId
    ,@CategoryId
    ,@Unit
    ,@Lesson
    ,@TeacherNotes
    ,@schoolContactId
)

if not exists (
    select 1
    from [Education].[ClassRecording-Active] classRecording
        inner join [Education].[SchoolContact-Active] schoolContact on classRecording.UploadedBy = schoolContact.Id
    where classRecording.Id = @Id
        and schoolContact.Id = @schoolContactId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction