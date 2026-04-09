create proc [EducationStudent].[ContentCompletedInsertByChapter] (
     @SessionId uniqueidentifier
    ,@ChapterId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @bookId uniqueidentifier = (select BookId from [Education].[Chapter-Active] where Id = @ChapterId)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[ContentCompleted] (
    Id
    ,UpdatedBy
    ,StudentId
    ,BookId
    ,DeviceTimestamp
)
values (
    @Id
    ,@SessionId
    ,@studentId
    ,@bookId
    ,sysdatetimeoffset()
)