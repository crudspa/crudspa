create proc [EducationStudent].[ObjectiveCompletedInsert] (
     @SessionId uniqueidentifier
    ,@ObjectiveId uniqueidentifier
    ,@DeviceTimestamp datetimeoffset
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

if (not exists(select Id from [Education].[ObjectiveCompleted-Active] where ObjectiveId = @ObjectiveId and StudentId = @studentId))
begin
    insert [Education].[ObjectiveCompleted] (
        Id
        ,UpdatedBy
        ,StudentId
        ,ObjectiveId
        ,DeviceTimestamp
    )
    values (
        @Id
        ,@SessionId
        ,@StudentId
        ,@ObjectiveId
        ,@DeviceTimestamp
    )
end