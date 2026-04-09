create proc [EducationSchool].[ClassRecordingDelete] (
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

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[ClassRecording] baseTable
    inner join [Education].[ClassRecording-Active] classRecording on classRecording.Id = baseTable.Id
    inner join [Education].[SchoolContact-Active] schoolContact on classRecording.UploadedBy = schoolContact.Id
where baseTable.Id = @Id
    and schoolContact.Id = @schoolContactId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction