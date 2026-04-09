create proc [FrameworkCore].[AccessDeniedInsert] (
     @SessionId uniqueidentifier
    ,@EventType nvarchar(50)
    ,@PermissionId uniqueidentifier = null
    ,@Method nvarchar(250)
) as

declare @id uniqueidentifier = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[AccessDenied] (
     Id
    ,Denied
    ,SessionId
    ,EventType
    ,PermissionId
    ,Method
)
values (
     @id
    ,@now
    ,@SessionId
    ,@EventType
    ,@PermissionId
    ,@Method
)