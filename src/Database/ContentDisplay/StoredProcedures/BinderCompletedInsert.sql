create proc [ContentDisplay].[BinderCompletedInsert] (
     @SessionId uniqueidentifier
    ,@BinderId uniqueidentifier
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
    insert [Content].[BinderCompleted] (
         Id
        ,UpdatedBy
        ,ContactId
        ,BinderId
        ,DeviceTimestamp
    )
    values (
         @Id
        ,@SessionId
        ,@contactId
        ,@BinderId
        ,@DeviceTimestamp
    )
end