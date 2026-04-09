create proc [EducationStudent].[PrefaceCompletedInsert] (
     @SessionId uniqueidentifier
    ,@BookId uniqueidentifier
    ,@DeviceTimestamp datetimeoffset
) as

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @Id uniqueidentifier = newid()

insert [Education].[PrefaceCompleted] (
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
    ,@BookId
    ,@DeviceTimestamp
)