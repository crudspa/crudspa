create proc [ContentDesign].[CommentInsert] (
     @SessionId uniqueidentifier
    ,@PostId uniqueidentifier
    ,@ThreadId uniqueidentifier
    ,@ParentId uniqueidentifier
    ,@Body nvarchar(max)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()
declare @byId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
set xact_abort on
begin transaction

if (@byId is null)
begin
    rollback transaction
    raiserror('Comment author not found for session', 16, 1)
    return
end

insert [Content].[Comment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ById
    ,PostId
    ,ThreadId
    ,ParentId
    ,Body
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@byId
    ,@PostId
    ,@ThreadId
    ,@ParentId
    ,@Body
)

commit transaction