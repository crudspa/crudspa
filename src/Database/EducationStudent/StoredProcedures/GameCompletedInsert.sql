create proc [EducationStudent].[GameCompletedInsert] (
     @SessionId uniqueidentifier
    ,@GameId uniqueidentifier
    ,@BookId uniqueidentifier
    ,@GameRunId uniqueidentifier
    ,@DeviceTimestamp datetimeoffset
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

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[GameCompleted] (
    Id
    ,UpdatedBy
    ,StudentId
    ,GameId
    ,BookId
    ,GameRunId
    ,DeviceTimestamp
)
values (
    @Id
    ,@SessionId
    ,@studentId
    ,@GameId
    ,@BookId
    ,@GameRunId
    ,@DeviceTimestamp
)