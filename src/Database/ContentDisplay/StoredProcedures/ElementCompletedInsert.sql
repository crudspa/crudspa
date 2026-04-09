create proc [ContentDisplay].[ElementCompletedInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@DeviceTimestamp datetimeoffset
    ,@Id uniqueidentifier output
) as

set @Id = newid()

declare @contactId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

if (@contactId is not null)
begin

insert [Content].[ElementCompleted] (
     Id
    ,UpdatedBy
    ,ContactId
    ,ElementId
    ,DeviceTimestamp
)
values (
     @Id
    ,@SessionId
    ,@contactId
    ,@ElementId
    ,@DeviceTimestamp
)

end