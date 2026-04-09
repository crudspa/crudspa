create proc [EducationStudent].[TrifoldCompletedInsert] (
     @SessionId uniqueidentifier
    ,@TrifoldId uniqueidentifier
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

insert [Education].[TrifoldCompleted] (
    Id
    ,UpdatedBy
    ,StudentId
    ,TrifoldId
    ,DeviceTimestamp
)
values (
    @Id
    ,@SessionId
    ,@studentId
    ,@TrifoldId
    ,@DeviceTimestamp
)