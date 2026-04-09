create proc [FrameworkCore].[AccessCodeInsert] (
     @SessionId uniqueidentifier
    ,@UserId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Code nvarchar(40)
    ,@Expires datetimeoffset(7)
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[AccessCode] (
    Id
    ,SessionId
    ,UserId
    ,PortalId
    ,Code
    ,Expires
)
values (
    newid()
    ,@SessionId
    ,@UserId
    ,@PortalId
    ,@Code
    ,@Expires
)