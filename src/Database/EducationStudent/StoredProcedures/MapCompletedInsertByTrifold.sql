create proc [EducationStudent].[MapCompletedInsertByTrifold] (
     @SessionId uniqueidentifier
    ,@TrifoldId uniqueidentifier
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

declare @bookId uniqueidentifier = (select BookId from [Education].[Trifold-Active] where Id = @TrifoldId)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[MapCompleted] (
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